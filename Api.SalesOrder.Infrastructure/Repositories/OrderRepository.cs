using Api.SalesOrder.Application.Interfaces;
using Api.SalesOrder.Domain.Entities;
using Api.SalesOrder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.SalesOrder.Infrastructure.Repositories
{
    public class OrderRepository(ApplicationDbContext _context) : IOrderRepository
    {
        public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetByCustomerAndDateAsync(
            string customerEmail,
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.CustomerEmail == customerEmail &&
                           o.OrderDate.Date == date.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? customerFilter = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerFilter))
            {
                query = query.Where(o => o.CustomerName.Contains(customerFilter));
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= endDate.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
            return order;
        }

        public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            _context.Orders.Update(order);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Order order, string deletedBy, CancellationToken cancellationToken = default)
        {
            // Logical erase because order data should be important
            order.IsDeleted = true;
            order.UpdatedBy = deletedBy;
            _context.Orders.Update(order);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders.AnyAsync(o => o.Id == id, cancellationToken);
        }
    }
}
