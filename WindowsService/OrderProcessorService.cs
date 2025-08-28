using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsService
{
    public class OrderProcessorService : BackgroundService
    {
        private readonly ILogger<OrderProcessorService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private Timer _timer;

        public OrderProcessorService(ILogger<OrderProcessorService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? "Server=(localdb)\\mssqllocaldb;Database=dinet-staging-sql;Trusted_Connection=True;";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Order Processor Service is starting.");

            // Run the processor every 30 seconds
            _timer = new Timer(ProcessOrders, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogInformation("Order Processor Service is stopping.");
        }

        private void ProcessOrders(object state)
        {
            _logger.LogInformation("Processing orders at: {time}", DateTimeOffset.Now);

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Get pending orders older than 5 minutes
                    var pendingOrderIds = new List<int>();

                    using (var command = new SqlCommand(
                        @"SELECT Id FROM Orders 
                          WHERE Status = 'Pending' 
                          AND DATEDIFF(MINUTE, CreatedAt, GETDATE()) > 5",
                        connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pendingOrderIds.Add((int)reader["Id"]);
                            }
                        }
                    }

                    // Update orders to Processing status
                    foreach (var orderId in pendingOrderIds)
                    {
                        using (var command = new SqlCommand(
                            @"UPDATE Orders 
                              SET Status = 'Processing', 
                                  UpdatedAt = GETDATE(), 
                                  UpdatedBy = 'BackgroundService'
                              WHERE Id = @OrderId",
                            connection))
                        {
                            command.Parameters.AddWithValue("@OrderId", orderId);
                            command.ExecuteNonQuery();

                            _logger.LogInformation("Order {OrderId} marked as Processing", orderId);
                        }
                    }

                    // Process orders in Processing status (older than 10 minutes)
                    var processingOrderIds = new List<int>();

                    using (var command = new SqlCommand(
                        @"SELECT Id FROM Orders 
                          WHERE Status = 'Processing' 
                          AND DATEDIFF(MINUTE, UpdatedAt, GETDATE()) > 10",
                        connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                processingOrderIds.Add((int)reader["Id"]);
                            }
                        }
                    }

                    // Mark as Processed
                    foreach (var orderId in processingOrderIds)
                    {
                        using (var command = new SqlCommand(
                            @"UPDATE Orders 
                              SET Status = 'Processed', 
                                  UpdatedAt = GETDATE(), 
                                  UpdatedBy = 'BackgroundService'
                              WHERE Id = @OrderId",
                            connection))
                        {
                            command.Parameters.AddWithValue("@OrderId", orderId);
                            command.ExecuteNonQuery();

                            _logger.LogInformation("Order {OrderId} marked as Processed", orderId);
                        }
                    }

                    if (pendingOrderIds.Count == 0 && processingOrderIds.Count == 0)
                    {
                        _logger.LogInformation("No orders to process");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing orders");
            }
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
