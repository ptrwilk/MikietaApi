namespace MikietaApi.Models;

public class ProductModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string[] Ingredients { get; set; }
    public required IEnumerable<IngredientModel> Ingredientss { get; set; }
    public double Price { get; set; }
    public ProductType ProductType { get; set; }
    public int Quantity { get; set; }
}