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