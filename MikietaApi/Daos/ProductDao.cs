namespace MikietaApi.Daos;

public class ProductDao
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IngredientDao[] Ingredients { get; set; }
    public double Price { get; set; }
    public ProductDaoType ProductType { get; set; }
}