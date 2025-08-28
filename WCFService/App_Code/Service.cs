using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Service : IService
{
    private readonly string _connectionString;

    public Service()
    {
        // In a real WCF service, this would come from app.config
        _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString
            ?? "Server=(localdb)\\mssqllocaldb;Database=SalesOrderDB;Trusted_Connection=True;";
    }

    public OrderRegistrationResponse RegistrarOrden(OrderRegistrationRequest request)
    {
        try
        {
            // Validate request
            if (string.IsNullOrEmpty(request.CustomerName))
            {
                return new OrderRegistrationResponse
                {
                    Success = false,
                    Message = "Customer name is required",
                    ErrorCode = "VALIDATION_ERROR"
                };
            }

            if (request.Details == null || request.Details.Count == 0)
            {
                return new OrderRegistrationResponse
                {
                    Success = false,
                    Message = "Order must have at least one detail",
                    ErrorCode = "VALIDATION_ERROR"
                };
            }

            // Simulate database insertion using ADO.NET (legacy approach)
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert order header
                        var orderId = 0;
                        using (var command = new SqlCommand(
                            @"INSERT INTO Orders (CustomerName, CustomerEmail, OrderDate, Total, Status, CreatedAt, CreatedBy)
                                  OUTPUT INSERTED.Id
                                  VALUES (@CustomerName, @CustomerEmail, @OrderDate, @Total, @Status, @CreatedAt, @CreatedBy)",
                            connection, transaction))
                        {
                            command.Parameters.AddWithValue("@CustomerName", request.CustomerName);
                            command.Parameters.AddWithValue("@CustomerEmail", request.CustomerEmail);
                            command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                            command.Parameters.AddWithValue("@Total", request.Details.Sum(d => d.Subtotal));
                            command.Parameters.AddWithValue("@Status", "Pending");
                            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                            command.Parameters.AddWithValue("@CreatedBy", "WCF_Service");

                            orderId = (int)command.ExecuteScalar();
                        }

                        // Insert order details
                        foreach (var detail in request.Details)
                        {
                            using (var command = new SqlCommand(
                                @"INSERT INTO OrderDetails (OrderId, ProductName, ProductCode, Quantity, UnitPrice, Subtotal, CreatedAt, CreatedBy)
                                      VALUES (@OrderId, @ProductName, @ProductCode, @Quantity, @UnitPrice, @Subtotal, @CreatedAt, @CreatedBy)",
                                connection, transaction))
                            {
                                command.Parameters.AddWithValue("@OrderId", orderId);
                                command.Parameters.AddWithValue("@ProductName", detail.ProductName);
                                command.Parameters.AddWithValue("@ProductCode", detail.ProductCode);
                                command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                                command.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);
                                command.Parameters.AddWithValue("@Subtotal", detail.Quantity * detail.UnitPrice);
                                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                                command.Parameters.AddWithValue("@CreatedBy", "WCF_Service");

                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();

                        return new OrderRegistrationResponse
                        {
                            Success = true,
                            Message = "Order registered successfully",
                            OrderId = orderId
                        };
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return new OrderRegistrationResponse
            {
                Success = false,
                Message = "Error registering order" + ex.Message,
                ErrorCode = "SYSTEM_ERROR"
            };
        }
    }

    public OrderStatusResponse ConsultarEstadoOrden(int orderId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(
                    "SELECT Status, UpdatedAt FROM Orders WHERE Id = @OrderId",
                    connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new OrderStatusResponse
                            {
                                Found = true,
                                Status = reader["Status"].ToString() ?? "Unknown",
                                LastUpdate = reader["UpdatedAt"] as DateTime?
                            };
                        }
                    }
                }
            }

            return new OrderStatusResponse
            {
                Found = false,
                Status = "Not Found"
            };
        }
        catch
        {
            return new OrderStatusResponse
            {
                Found = false,
                Status = "Error"
            };
        }
    }

    public List<OrderContract> ObtenerOrdenesPendientes()
    {
        var orders = new List<OrderContract>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Get pending orders
                using (var command = new SqlCommand(
                    @"SELECT o.Id, o.CustomerName, o.CustomerEmail, o.OrderDate, o.Total, o.Status
                          FROM Orders o
                          WHERE o.Status = 'Pending'
                          ORDER BY o.OrderDate",
                    connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new OrderContract
                            {
                                OrderId = (int)reader["Id"],
                                CustomerName = reader["CustomerName"].ToString() ?? "",
                                CustomerEmail = reader["CustomerEmail"].ToString() ?? "",
                                OrderDate = (DateTime)reader["OrderDate"],
                                Total = (decimal)reader["Total"],
                                Status = reader["Status"].ToString() ?? ""
                            };

                            orders.Add(order);
                        }
                    }
                }

                // Get details for each order
                foreach (var order in orders)
                {
                    using (var command = new SqlCommand(
                        @"SELECT ProductName, ProductCode, Quantity, UnitPrice, Subtotal
                              FROM OrderDetails
                              WHERE OrderId = @OrderId",
                        connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", order.OrderId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                order.Details.Add(new OrderDetailContract
                                {
                                    ProductName = reader["ProductName"].ToString() ?? "",
                                    ProductCode = reader["ProductCode"].ToString() ?? "",
                                    Quantity = (int)reader["Quantity"],
                                    UnitPrice = (decimal)reader["UnitPrice"],
                                    Subtotal = (decimal)reader["Subtotal"]
                                });
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // In production, log the error
            Console.WriteLine("Error getting pending orders:" + ex.Message);
        }

        return orders;
    }
}
