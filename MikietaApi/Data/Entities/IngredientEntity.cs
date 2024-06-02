using System.ComponentModel.DataAnnotations;

namespace MikietaApi.Data.Entities;

public class IngredientEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public int Index { get; set; }
    public double PriceSmall { get; set; }
    public double PriceMedium { get; set; }
    public double PriceLarge { get; set; }
    public double[] Prices => new []{ PriceSmall, PriceMedium, PriceLarge };
    public ICollection<ProductEntity> Products { get; set; }

}