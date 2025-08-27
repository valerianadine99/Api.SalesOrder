using Api.SalesOrder.SharedKernel.Entities;

namespace Api.SalesOrder.SharedKernel.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
