using System.ComponentModel.DataAnnotations;

namespace MikietaApi.Data.Entities;

public class IngredientEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public int Index { get; set; }
    public decimal PriceSmall { get; set; }
    public decimal PriceMedium { get; set; }
    public decimal PriceLarge { get; set; }
    public decimal[] Prices => new []{ PriceSmall, PriceMedium, PriceLarge };
    public ICollection<ProductEntity> Products { get; set; }
}