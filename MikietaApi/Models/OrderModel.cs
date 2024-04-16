namespace MikietaApi.Models;

public class ProcessingPersonalData
{
    public bool? Email { get; set; }
    public bool? Sms { get; set; }
}

public class OrderModel
{
    public int[] ProductIds { get; set; } = null!;
    public DateTime? DeliveryTiming { get; set; }
    public bool? DeliveryRightAway { get; set; }
    public DeliveryMethodType DeliveryMethod { get; set; }
    public string? Comments { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Nip { get; set; }
    public ProcessingPersonalData? ProcessingPersonalData { get; set; }
}