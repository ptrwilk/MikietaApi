namespace MikietaApi.SendEmail.Order;

public class OrderProductFragmentModel : EmailSenderModelBase
{
    public string Name { get; set; } = null!;
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string[]? Ingredients { get; set; }
}