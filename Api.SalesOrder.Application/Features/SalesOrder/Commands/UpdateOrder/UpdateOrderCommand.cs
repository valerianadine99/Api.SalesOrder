using Api.SalesOrder.Application.DTOs;
using MediatR;

namespace Api.SalesOrder.Application.Features.SalesOrder.Commands.UpdateOrder
{
    public class UpdateOrderCommand : IRequest<OrderDto>
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<CreateOrderDetailDto> OrderDetails { get; set; } = new();
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
