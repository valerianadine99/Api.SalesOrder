using Api.SalesOrder.Domain.Entities;
using Api.SalesOrder.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace Api.SalesOrder.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private IDbContextTransaction? _currentTransaction;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Seed data for testing
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    OrderDate = DateTime.UtcNow.AddDays(-5),
                    CustomerName = "John Doe",
                    CustomerEmail = "john.doe@example.com",
                    Total = 1500.00m,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    CreatedBy = "System"
                }
            );

            modelBuilder.Entity<OrderDetail>().HasData(
                new OrderDetail
                {
                    Id = 1,
                    OrderId = 1,
                    ProductName = "Laptop",
                    ProductCode = "TECH-001",
                    Quantity = 1,
                    UnitPrice = 1000.00m,
                    Subtotal = 1000.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    CreatedBy = "System"
                },
                new OrderDetail
                {
                    Id = 2,
                    OrderId = 1,
                    ProductName = "Mouse",
                    ProductCode = "TECH-002",
                    Quantity = 2,
                    UnitPrice = 250.00m,
                    Subtotal = 500.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    CreatedBy = "System"
                }
            );
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = await Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                await _currentTransaction?.CommitAsync()!;
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _currentTransaction?.RollbackAsync()!;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

    }
}
