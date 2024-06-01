namespace MikietaApi.Models;

public class ProcessingPersonalData
{
    public bool? Email { get; set; }
    public bool? Sms { get; set; }
}

public class OrderModel
{
    public required int[] ProductIds { get; set; }
    public DateTime? DeliveryTiming { get; set; }
    public bool? DeliveryRightAway { get; set; }
    public DeliveryMethodType DeliveryMethod { get; set; }
    public string? Comments { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public string? Nip { get; set; }
    public string? Street { get; set; }
    public string? HomeNumber { get; set; }
    public string? City { get; set; }
    public string? FlatNumber { get; set; }
    public string? Floor { get; set; }
    public ProcessingPersonalData? ProcessingPersonalData { get; set; }
}