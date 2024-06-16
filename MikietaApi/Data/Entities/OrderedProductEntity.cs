using System.ComponentModel.DataAnnotations;
using MikietaApi.Models;

namespace MikietaApi.Data.Entities;

public class OrderedProductEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public string Name { get; set; } = null!;
    public double Price { get; set; }
    public ProductType ProductType { get; set; }
    public PizzaType? PizzaType { get; set; }
    public string? Description { get; set; }
    public int Index { get; set; }
    public ICollection<OrderedProductOrderedIngredientEntity> OrderedProductOrderedIngredients { get; set; }
    public ICollection<OrderOrderedProductEntity> OrderOrderedProducts { get; set; }
}