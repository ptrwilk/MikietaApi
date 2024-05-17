namespace MikietaApi.Models;

public class ProcessingPersonalData
{
    public bool? Email { get; set; }
    public bool? Sms { get; set; }
}

public class ProductQuantityModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class OrderModel
{
    public ProductQuantityModel[] ProductQuantities { get; set; } = null!;
    public DateTime? DeliveryTiming { get; set; }
    public bool? DeliveryRightAway { get; set; }
    public DeliveryMethodType DeliveryMethod { get; set; }
    public string? Comments { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Nip { get; set; }
    public string? Street { get; set; }
    public string? HomeNumber { get; set; }
    public string? City { get; set; }
    public string? FlatNumber { get; set; }
    public string? Floor { get; set; }
    public ProcessingPersonalData? ProcessingPersonalData { get; set; }
}

public class AdminAddressModel
{
    public string? City { get; set; }
    public string? HomeNumber { get; set; }
    public string? Street { get; set; }
    public string? FlatNumber { get; set; }
    public string? Floor { get; set; }
    public string Text => $"{City} {Street} {HomeNumber} {FlatNumber} {Floor}".Trim();
}

public class AdminOrderModel
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = null!;
    public string AddressText { get; set; } = null!;
    public AdminAddressModel Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public bool Payed { get; set; }
    public int TotalProducts { get; set; }
    public int CompletedProducts { get; set; }
    public OrderStatusType Status { get; set; }
    public bool OnSitePickup { get; set; }
    public double Cost { get; set; }
    public DateTime DeliveryAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderStatusModel
{
    public OrderStatusType Status { get; set; }
    public DateTime DeliveryAt { get; set; }
}