namespace Api.SalesOrder.Application.DTOs
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<CreateOrderDetailDto> OrderDetails { get; set; } = new();
    }
}
