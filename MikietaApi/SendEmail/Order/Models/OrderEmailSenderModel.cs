namespace MikietaApi.SendEmail.Order;

public class OrderEmailSenderModel : EmailSenderModelBase
{
    public string Link { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public bool Delivery { get; set; }
    public bool TransferPaid { get; set; }
    public double DeliveryCost { get; set; }
    public OrderProductFragmentModel[] Products { get; set; } = null!;
}