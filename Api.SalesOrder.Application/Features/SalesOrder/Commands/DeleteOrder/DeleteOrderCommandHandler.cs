using Api.SalesOrder.Application.Interfaces;
using MediatR;

namespace Api.SalesOrder.Application.Features.SalesOrder.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<DeleteOrderCommand, bool>
    {
        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.Id, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {request.Id} not found");

            await _unitOfWork.Orders.DeleteAsync(order, request.DeletedBy, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
