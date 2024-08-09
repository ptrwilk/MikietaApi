using System.Text.Json;
using System.Text.Json.Serialization;
using MikietaApi.Models;

namespace MikietaApi.Data;

public class ProductPlainModel
{
    [JsonPropertyName("pizzas")]
    public PizzaPlainModel[] Pizzas { get; set; } = null!;

    [JsonPropertyName("dinners")]
    public OtherPlainModel[] Dinners { get; set; } = null!;
    [JsonPropertyName("macarons")]
    public OtherPlainModel[] Macarons { get; set; } = null!; 
    [JsonPropertyName("salads")]
    public OtherPlainModel[] Salads { get; set; } = null!;
    [JsonPropertyName("deserts")]
    public OtherPlainModel[] Deserts { get; set; } = null!;
    [JsonPropertyName("drinks")]
    public DrinkPlainModel[] Drinks { get; set; } = null!;
}

public class PizzaPlainModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("ingredients")]
    public string Ingredients { get; set; } = null!;
    [JsonPropertyName("medium_price")]
    public string MediumPrice { get; set; } = null!;
    [JsonPropertyName("large_price")]
    public string LargePrice { get; set; } = null!;
}

public class OtherPlainModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;
    [JsonPropertyName("price")]
    public string Price { get; set; } = null!;
}

public class DrinkPlainModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("price")]
    public string Price { get; set; } = null!;
}

public class ProductParsedModel
{
    public string Name { get; set; } = null!;
    public string[]? Ingredients { get; set; }
    public string? Description { get; set; }
    public ProductType ProductType { get; set; }
    public decimal? MediumPrice { get; set; }
    public decimal? LargePrice { get; set; }
    public decimal? Price { get; set; }
}

public class ProductsParser
{
    public ProductParsedModel[] Parse()
    {
        var text = File.ReadAllText(@"Data\Seeder\Scripts\Products\products.json");

        var model = JsonSerializer.Deserialize<ProductPlainModel>(text)!;

        var res = new List<ProductParsedModel>();

        foreach (var pizza in model.Pizzas)
        {
            res.Add(CreateProduct(pizza));
        }
        
        foreach (var dinner in model.Dinners)
        {
            res.Add(CreateProduct(dinner, ProductType.Dinner));
        }
        
        foreach (var macaron in model.Macarons)
        {
            res.Add(CreateProduct(macaron, ProductType.Macaron));
        }
        
        foreach (var salad in model.Salads)
        {
            res.Add(CreateProduct(salad, ProductType.Salad));
        }
        
        foreach (var desert in model.Deserts)
        {
            res.Add(CreateProduct(desert, ProductType.Desert));
        }
        
        foreach (var drink in model.Drinks)
        {
            res.Add(CreateDrink(drink));
        }

        return res.ToArray();
    }

    private ProductParsedModel CreateProduct(PizzaPlainModel model)
    {
        var ingredients = model.Ingredients.Split(",").Select(x => x.Trim());
        return new ProductParsedModel
        {
            Ingredients = ingredients.ToArray(),
            ProductType = ProductType.Pizza,
            Name = model.Name,
            MediumPrice = decimal.TryParse(model.MediumPrice, out var mediumPrice) ? mediumPrice : null,
            LargePrice = decimal.TryParse(model.LargePrice, out var largePrice) ? largePrice : null,
        };
    }
    
    private ProductParsedModel CreateProduct(OtherPlainModel model, ProductType productType)
    {
        return new ProductParsedModel
        {
            ProductType = productType,
            Name = model.Name,
            Price = decimal.Parse(model.Price),
            Description = model.Description
        };
    }
    
    private ProductParsedModel CreateDrink(DrinkPlainModel model)
    {
        return new ProductParsedModel
        {
            ProductType = ProductType.Drink,
            Name = model.Name,
            Price = decimal.Parse(model.Price),
        };
    }
}