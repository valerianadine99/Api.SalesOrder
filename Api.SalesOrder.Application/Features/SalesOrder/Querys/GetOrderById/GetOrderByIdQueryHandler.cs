using Api.SalesOrder.Application.DTOs;
using Api.SalesOrder.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Api.SalesOrder.Application.Features.SalesOrder.Querys.GetOrderById
{
    public class GetOrderByIdQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(request.Id, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {request.Id} not found");

            return _mapper.Map<OrderDto>(order);
        }
    }
}
