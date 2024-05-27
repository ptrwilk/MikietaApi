using System.ComponentModel.DataAnnotations;

namespace MikietaApi.Data.Entities;

public class ProductEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<IngredientEntity> Ingredients { get; set; }
    public double Price { get; set; }
    public ProductTypeEntity ProductType { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<OrderProductEntity> OrderProducts { get; set; }
}