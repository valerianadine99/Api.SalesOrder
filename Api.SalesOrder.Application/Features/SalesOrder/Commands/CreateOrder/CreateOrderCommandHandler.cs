using Api.SalesOrder.Application.DTOs;
using Api.SalesOrder.Application.Interfaces;
using Api.SalesOrder.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Api.SalesOrder.Application.Features.SalesOrder.Commands.CreateOrder
{
    public class CreateOrderCommandHandler(
        IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Before creating order we should make validations of stock, price but its not part of this test

            // Check if customer already has an order on the same date
            var existingOrders = await _unitOfWork.Orders.GetByCustomerAndDateAsync(
                request.CustomerEmail,
                request.OrderDate.Date,
                cancellationToken);

            if (existingOrders.Any())
            {
                throw new InvalidOperationException(
                    $"Customer with email {request.CustomerEmail} already has an order on {request.OrderDate.Date:yyyy-MM-dd}");
            }

            var order = new Order
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                OrderDate = request.OrderDate,
                Status = Domain.Enums.OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System" // Should come from current user context
            };

            foreach (var detailDto in request.OrderDetails)
            {
                var detail = new OrderDetail
                {
                    ProductName = detailDto.ProductName,
                    ProductCode = detailDto.ProductCode,
                    Quantity = detailDto.Quantity,
                    UnitPrice = detailDto.UnitPrice,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy // UserId that have created order
                };
                order.AddDetail(detail);
            }

            var createdOrder = await _unitOfWork.Orders.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<OrderDto>(createdOrder);

        }
    }
}
