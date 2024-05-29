using System.ComponentModel.DataAnnotations;

namespace MikietaApi.Data.Entities;

public class IngredientEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public int Index { get; set; }
    public ICollection<ProductEntity> Products { get; set; }
}