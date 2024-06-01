using System.ComponentModel.DataAnnotations;
using MikietaApi.Models;

namespace MikietaApi.Data.Entities;

public class ProductEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<IngredientEntity> Ingredients { get; set; }
    public double Price { get; set; }
    public ProductType ProductType { get; set; }
    public PizzaType? PizzaType { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? ImageId { get; set; }
    public int Index { get; set; }
    public ICollection<OrderProductEntity> OrderProducts { get; set; }
}