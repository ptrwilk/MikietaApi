namespace MikietaApi.Models;

public class IngredientModel
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public decimal PriceSmall { get; set; }
    public decimal PriceMedium { get; set; }
    public decimal PriceLarge { get; set; }
    public decimal[] Prices => new []{ PriceSmall, PriceMedium, PriceLarge };
}