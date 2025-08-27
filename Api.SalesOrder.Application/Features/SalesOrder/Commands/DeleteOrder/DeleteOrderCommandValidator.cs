using FluentValidation;

namespace Api.SalesOrder.Application.Features.SalesOrder.Commands.DeleteOrder
{
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(x => x.Id)
    .GreaterThan(0).WithMessage("Invalid order ID");
        }
    }
}
