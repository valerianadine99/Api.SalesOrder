using Api.SalesOrder.Application.DTOs;
using Api.SalesOrder.Application.Interfaces;
using Api.SalesOrder.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Api.SalesOrder.Application.Features.SalesOrder.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<UpdateOrderCommand, OrderDto>
    {
        public async Task<OrderDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            //This is part of the required crud but current update from order should be the status change
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(request.Id, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {request.Id} not found");

            // Check if changing date would conflict with another order
            if (order.OrderDate.Date != request.OrderDate.Date ||
                order.CustomerName != request.CustomerName)
            {
                var existingOrders = await _unitOfWork.Orders.GetByCustomerAndDateAsync(
                    request.CustomerEmail,
                    request.OrderDate.Date,
                    cancellationToken);

                if (existingOrders.Any(o => o.Id != request.Id))
                {
                    throw new InvalidOperationException(
                        $"Customer {request.CustomerName} already has an order on {request.OrderDate.Date:yyyy-MM-dd}");
                }
            }

            order.CustomerName = request.CustomerName;
            order.CustomerEmail = request.CustomerEmail;
            order.OrderDate = request.OrderDate;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = "System";

            // Clear and recreate details
            order.OrderDetails.Clear();
            foreach (var detailDto in request.OrderDetails)
            {
                var detail = new OrderDetail
                {
                    ProductName = detailDto.ProductName,
                    ProductCode = detailDto.ProductCode,
                    Quantity = detailDto.Quantity,
                    UnitPrice = detailDto.UnitPrice,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                order.AddDetail(detail);
            }

            await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<OrderDto>(order);
        }
    }
}
