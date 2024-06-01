namespace MikietaApi.Data.Entities;

public class IngredientEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<ProductEntity>? Products { get; set; }
}