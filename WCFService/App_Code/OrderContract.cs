using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

[DataContract]
public class OrderContract
{
    [DataMember]
    public int OrderId { get; set; }

    [DataMember]
    public string CustomerName { get; set; }

    [DataMember]
    public string CustomerEmail { get; set; }

    [DataMember]
    public DateTime OrderDate { get; set; }

    [DataMember]
    public decimal Total { get; set; }

    [DataMember]
    public string Status { get; set; }

    [DataMember]
    public List<OrderDetailContract> Details { get; set; }
}

[DataContract]
public class OrderDetailContract
{
    [DataMember]
    public string ProductName { get; set; }

    [DataMember]
    public string ProductCode { get; set; }

    [DataMember]
    public int Quantity { get; set; }

    [DataMember]
    public decimal UnitPrice { get; set; }

    [DataMember]
    public decimal Subtotal { get; set; }
}

[DataContract]
public class OrderRegistrationRequest
{
    [DataMember]
    public string CustomerName { get; set; }

    [DataMember]
    public string CustomerEmail { get; set; }

    [DataMember]
    public List<OrderDetailContract> Details { get; set; }
}

[DataContract]
public class OrderRegistrationResponse
{
    [DataMember]
    public bool Success { get; set; }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public int? OrderId { get; set; }

    [DataMember]
    public string? ErrorCode { get; set; }
}

[DataContract]
public class OrderStatusResponse
{
    [DataMember]
    public bool Found { get; set; }

    [DataMember]
    public string Status { get; set; }

    [DataMember]
    public DateTime? LastUpdate { get; set; }
}