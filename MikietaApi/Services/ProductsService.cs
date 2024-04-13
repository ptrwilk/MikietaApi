using MikietaApi.Models;
using MikietaApi.Repositories;

namespace MikietaApi.Services;

public interface IProductsService
{
    ProductModel[] Get();
}

public class ProductsService : IProductsService
{
    private readonly IProductsRepository _repository;

    public ProductsService(IProductsRepository repository)
    {
        _repository = repository;
    }

    public ProductModel[] Get()
    {
        return new[]
        {
            CreateDrink("Kozel Lezak", 10),
            CreateDrink("Żywiec Klasyk", 11),
            CreateDrink("Grzaniec XXL 0.5L", 19),
            CreateDrink("Grzaniec Duży", 14),
            CreateDrink("Grzaniec Mały", 10),
            CreateProduct("CALZONE pieróg z nadzieniem",
                new[] { "w każdym pierożku, ser, sos czosnkowy + 4 dowolne składniki do wyboru" }, 27,
                ProductType.Snack),
            CreateProduct("Frytki Cieńkie Małe", new[] { "Porcja chrupiących frytek, ketchup" }, 8, ProductType.Snack),
            CreateProduct("Frytki Cieńkie Duże", new[] { "Porcja chrupiących frytek, ketchup" }, 10, ProductType.Snack),
            CreateProduct("Frytki Belgijskie Małe", new[] { "Porcja chrupiących frytek, ketchup" }, 8,
                ProductType.Snack),
            CreateProduct("Frytki Belgijskie Duże", new[] { "Porcja chrupiących frytek, ketchup" }, 10,
                ProductType.Snack),
            Margherita(17, ProductType.PizzaSmall), Cipolla(18, ProductType.PizzaSmall),
            Funghi(18, ProductType.PizzaSmall), Salami(18, ProductType.PizzaSmall),
            FunghiEProsciutto(18, ProductType.PizzaSmall),
            Margherita(22, ProductType.PizzaMedium), Cipolla(23, ProductType.PizzaMedium),
            Funghi(23, ProductType.PizzaMedium), Salami(26, ProductType.PizzaMedium),
            FunghiEProsciutto(26, ProductType.PizzaMedium),
            Margherita(33, ProductType.PizzaBig), Cipolla(34, ProductType.PizzaBig), Funghi(34, ProductType.PizzaBig),
            Salami(36, ProductType.PizzaBig), FunghiEProsciutto(36, ProductType.PizzaBig),
            CreateProduct("Czosnkowy", new []{"firmowy sos XXX idealnie komponujący się ze smakiem każdej naszej pizzy"}, 4, ProductType.Sauce),
            CreateProduct("Czosnkowo-szczypiorkowy", new []{"firmowy sos XXX, ketchup"}, 4, ProductType.Sauce),
            CreateProduct("Pomidorowy łagodny", new []{""}, 4, ProductType.Sauce)
        };
    }

    private ProductModel CreateDrink(string name, double price, string[]? ingredients = null) =>
        new()
        {
            Id = 1,
            Ingredients = ingredients,
            Name = name,
            Price = price,
            ProductType = ProductType.Drink
        };

    private ProductModel CreateProduct(string name, string[] ingredients, double price, ProductType productType) =>
        new()
        {
            Id = 1,
            Ingredients = ingredients,
            Name = name,
            Price = price,
            ProductType = productType
        };

    private ProductModel Margherita(double price, ProductType productType) =>
        CreateProduct("Margherita", new[] { "ser", "sos pomidorowy" }, price, productType);

    private ProductModel Cipolla(double price, ProductType productType) =>
        CreateProduct("Cipolla", new[] { "ser", "sos pomidorowy", "cebula" }, price, productType);

    private ProductModel Funghi(double price, ProductType productType) =>
        CreateProduct("Funghi", new[] { "ser", "sos pomidorowy", "pieczarki" }, price, productType);

    private ProductModel Salami(double price, ProductType productType) => CreateProduct("Salami",
        new[] { "ser", "sos pomidorowy", "salami" }, price, productType);

    private ProductModel FunghiEProsciutto(double price, ProductType productType) => CreateProduct(
        "Funghi e Prosciutto",
        new[] { "ser", "sos pomidorowy", "szynka", "pieczarki" }, price, productType);
}