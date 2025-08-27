using Api.SalesOrder.Application.DTOs;
using MediatR;

namespace Api.SalesOrder.Application.Features.SalesOrder.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
        public string CreatedBy { get; set; } = string.Empty;
    }
}
