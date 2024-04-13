namespace MikietaApi.Models;

public class ProductModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string[] Ingredients { get; set; }
    public double Price { get; set; }
    public ProductType ProductType { get; set; }
}