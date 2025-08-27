using Api.SalesOrder.Application.DTOs;
using MediatR;

namespace Api.SalesOrder.Application.Features.SalesOrder.Querys.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderDto>
    {
        public int Id { get; set; }
    }
}
