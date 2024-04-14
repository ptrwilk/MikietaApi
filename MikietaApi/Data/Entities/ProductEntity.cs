namespace MikietaApi.Data.Entities;

public class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<IngredientEntity> Ingredients { get; set; }
    public double Price { get; set; }
    public ProductTypeEntity ProductType { get; set; }
    public string? Description { get; set; }
}