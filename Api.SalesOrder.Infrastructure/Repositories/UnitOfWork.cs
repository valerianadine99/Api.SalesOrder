using Api.SalesOrder.Application.Interfaces;
using Api.SalesOrder.Infrastructure.Persistence;

namespace Api.SalesOrder.Infrastructure.Repositories
{
    public class UnitOfWork(ApplicationDbContext _context, IOrderRepository? _orderRepository) : IUnitOfWork
    {
        public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
