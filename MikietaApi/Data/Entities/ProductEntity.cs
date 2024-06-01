using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MikietaApi.Data.Entities.Enums;

namespace MikietaApi.Data.Entities;

public class ProductEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<IngredientEntity>? Ingredients { get; set; }
    public double Price { get; set; }
    public ProductTypeEntity ProductType { get; set; }
    public string? Description { get; set; }
    public ICollection<OrderEntity>? Orders { get; set; }
}