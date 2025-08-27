using MediatR;

namespace Api.SalesOrder.Application.Features.SalesOrder.Commands.DeleteOrder
{
    public class DeleteOrderCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string DeletedBy { get; set; }
    }
}
