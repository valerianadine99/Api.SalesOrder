using Api.SalesOrder.Domain.Enums;
using Api.SalesOrder.SharedKernel.Entities;

namespace Api.SalesOrder.Domain.Entities
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }

        // Navigation property
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        // Business rules
        public void CalculateTotal()
        {
            Total = OrderDetails.Sum(x => x.Subtotal);
        }

        public bool IsValid()
        {
            return OrderDetails.Any() &&
                   OrderDetails.All(d => d.Quantity > 0 && d.UnitPrice >= 0);
        }

        public void AddDetail(OrderDetail detail)
        {
            if (detail == null)
                throw new ArgumentNullException(nameof(detail));

            if (detail.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            if (detail.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative");

            detail.CalculateSubtotal();
            OrderDetails.Add(detail);
            CalculateTotal();
        }
    }
}
