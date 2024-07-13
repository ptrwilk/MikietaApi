using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MikietaApi.Models;

namespace MikietaApi.Data.Entities;

public class ProductEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<IngredientEntity> Ingredients { get; set; }
    public ICollection<PizzaSizeEntity> Sizes { get; set; }
    public decimal? Price { get; set; }
    public ProductType ProductType { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    [ForeignKey(nameof(ImageId))]
    public Guid? ImageId { get; set; }
    public ImageEntity? Image { get; set; }
    public int Index { get; set; }

    public decimal? GetPrice(PizzaType? type)
    {
        return type.HasValue ? Sizes.First(x => x.Size == type).Price : Price;
    }
}