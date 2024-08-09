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
    public decimal PriceMedium { get; set; }
    public decimal PriceLarge { get; set; }
    public decimal[] Prices => new []{ PriceMedium, PriceLarge };
    public ICollection<OrderedProductEntity> OrderedProducts { get; set; }
    public ICollection<OrderedProductOrderedIngredientEntity> OrderedProductOrderedIngredients { get; set; }
}