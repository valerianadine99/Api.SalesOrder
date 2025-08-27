namespace Api.SalesOrder.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1, //new orders
        Confirmed = 2, //order alredy payed
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }
}
