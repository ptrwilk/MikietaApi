namespace MikietaApi.SendEmail.Order;

public class OrderProductAdditionalIngredientModel
{
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
}

public class OrderProductFragmentModel : EmailSenderModelBase
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string[]? Ingredients { get; set; }
    public OrderProductAdditionalIngredientModel[]? AdditionalIngredients { get; set; }
}