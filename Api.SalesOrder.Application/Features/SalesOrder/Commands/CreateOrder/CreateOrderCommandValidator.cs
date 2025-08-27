using FluentValidation;

namespace Api.SalesOrder.Application.Features.SalesOrder.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required")
                .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters");

            RuleFor(x => x.CustomerEmail)
                .NotEmpty().WithMessage("Customer email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("Order date is required");

            RuleFor(x => x.OrderDetails)
                .NotEmpty().WithMessage("Order must have at least one detail")
                .Must(details => details != null && details.Count > 0)
                .WithMessage("Order must have at least one detail");

            RuleForEach(x => x.OrderDetails).ChildRules(detail =>
            {
                detail.RuleFor(d => d.ProductName)
                    .NotEmpty().WithMessage("Product name is required");

                detail.RuleFor(d => d.ProductCode)
                    .NotEmpty().WithMessage("Product code is required");

                detail.RuleFor(d => d.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than zero");

                detail.RuleFor(d => d.UnitPrice)
                    .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative");
            });
        }
    }
}