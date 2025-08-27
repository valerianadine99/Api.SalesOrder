using Api.SalesOrder.Domain.Entities;
using Api.SalesOrder.SharedKernel.Interfaces;

namespace Api.SalesOrder.Application.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByCustomerAndDateAsync(string customerEmail, DateTime date, CancellationToken cancellationToken = default);
        Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? customerFilter = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default);
        Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
        Task DeleteAsync(Order order, string deletedBy, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}
