namespace MikietaApi.Models;

public class IngredientModel
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public double PriceSmall { get; set; }
    public double PriceMedium { get; set; }
    public double PriceLarge { get; set; }
    public double[] Prices => new []{ PriceSmall, PriceMedium, PriceLarge };
}