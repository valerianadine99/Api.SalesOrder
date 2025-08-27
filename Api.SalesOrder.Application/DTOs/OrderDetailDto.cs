﻿namespace Api.SalesOrder.Application.DTOs
{
    public class OrderDetailDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
