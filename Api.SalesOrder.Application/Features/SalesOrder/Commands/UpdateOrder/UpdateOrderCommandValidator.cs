using FluentValidation;

namespace Api.SalesOrder.Application.Features.SalesOrder.Commands.UpdateOrder
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid order ID");

            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required")
                .MaximumLength(100);

            RuleFor(x => x.CustomerEmail)
                .NotEmpty().WithMessage("Customer email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.OrderDetails)
                .NotEmpty().WithMessage("Order must have at least one detail");
        }
    }
}
