using Api.SalesOrder.SharedKernel.Entities;

namespace Api.SalesOrder.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }

        // Navigation property
        public Order Order { get; set; } = null!;

        public void CalculateSubtotal()
        {
            Subtotal = Quantity * UnitPrice;
        }
    }
}
