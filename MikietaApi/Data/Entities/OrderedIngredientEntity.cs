using System.ComponentModel.DataAnnotations;

namespace MikietaApi.Data.Entities;

public class OrderedIngredientEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public IngredientEntity? Ingredient { get; set; }
    public string Name { get; set; } = null!;
    public int Index { get; set; }
    public double PriceSmall { get; set; }
    public double PriceMedium { get; set; }
    public double PriceLarge { get; set; }
    public double[] Prices => new []{ PriceSmall, PriceMedium, PriceLarge };
    public ICollection<OrderedProductEntity> OrderedProducts { get; set; }
    public ICollection<OrderedProductOrderedIngredientEntity> OrderedProductOrderedIngredients { get; set; }
}