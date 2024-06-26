using System.ComponentModel.DataAnnotations;
using MikietaApi.Models;

namespace MikietaApi.Data.Entities;

public class PizzaSizeEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;
    public PizzaType Size { get; set; }
    public double Price { get; set; }
}