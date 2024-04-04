var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddPolicy("MyPolicy",
        b => { b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }
    )
);

var app = builder.Build();

app.UseCors("MyPolicy");

app.MapGet("/", () => new[] { "a", "b", "c" });


app.MapGet("/menu", () =>
{
    return Results.Ok(new[]
    {
        CreateDrink("Kozel Lezak", 10),
        CreateDrink("Żywiec Klasyk", 11),
        CreateDrink("Grzaniec XXL 0.5L", 19),
        CreateDrink("Grzaniec Duży", 14),
        CreateDrink("Grzaniec Mały", 10),
        new ProductModel("CALZONE pieróg z nadzieniem", new []{"w każdym pierożku, ser, sos czosnkowy + 4 dowolne składniki do wyboru"}, 27, ProductType.Snack),
        new ProductModel("Frytki Cieńkie Małe", new []{"Porcja chrupiących frytek, ketchup"}, 8, ProductType.Snack),
        new ProductModel("Frytki Cieńkie Duże", new []{"Porcja chrupiących frytek, ketchup"}, 10, ProductType.Snack),
        new ProductModel("Frytki Belgijskie Małe", new []{"Porcja chrupiących frytek, ketchup"}, 8, ProductType.Snack),
        new ProductModel("Frytki Belgijskie Duże", new []{"Porcja chrupiących frytek, ketchup"}, 10, ProductType.Snack),
        Margherita(17, ProductType.PizzaSmall), Cipolla(18, ProductType.PizzaSmall), Funghi(18, ProductType.PizzaSmall), Salami(18, ProductType.PizzaSmall), FunghiEProsciutto(18, ProductType.PizzaSmall),
        Margherita(22, ProductType.PizzaMedium), Cipolla(23, ProductType.PizzaMedium), Funghi(23, ProductType.PizzaMedium), Salami(26, ProductType.PizzaMedium), FunghiEProsciutto(26, ProductType.PizzaMedium),
        Margherita(33, ProductType.PizzaBig), Cipolla(34, ProductType.PizzaBig), Funghi(34, ProductType.PizzaBig), Salami(36, ProductType.PizzaBig), FunghiEProsciutto(36, ProductType.PizzaBig),
        new ProductModel("Czosnkowy", new []{"firmowy sos XXX idealnie komponujący się ze smakiem każdej naszej pizzy"}, 4, ProductType.Sauce),
        new ProductModel("Czosnkowo-szczypiorkowy", new []{"firmowy sos XXX, ketchup"}, 4, ProductType.Sauce),
        new ProductModel("Pomidorowy łagodny", new []{""}, 4, ProductType.Sauce)
    });
});

app.MapPost("/payment", (PaymentModel model) => Results.Ok(Guid.NewGuid()));

app.MapGet("/delivery/{deliveryId}", (Guid deliveryId) =>
{
    return Results.Ok($"Test {deliveryId}");
});

app.Run();

static ProductModel Margherita(double price, ProductType productType) => new ProductModel("Margherita", new[] { "ser", "sos pomidorowy" }, price, productType);

static ProductModel Cipolla(double price, ProductType productType) =>
    new ProductModel("Cipolla", new[] { "ser", "sos pomidorowy", "cebula" }, price, productType);

static ProductModel Funghi(double price, ProductType productType) =>
    new ProductModel("Funghi", new[] { "ser", "sos pomidorowy", "pieczarki" }, price, productType);

static ProductModel Salami(double price, ProductType productType) => new ProductModel("Salami", new[] { "ser", "sos pomidorowy", "salami" }, price, productType);

static ProductModel FunghiEProsciutto(double price, ProductType productType) => new ProductModel("Funghi e Prosciutto",
    new[] { "ser", "sos pomidorowy", "szynka", "pieczarki" }, price, productType);

ProductModel CreateDrink(string name, double price, string[]? ingredients = null) =>
    new ProductModel(name, ingredients ?? Array.Empty<string>(), price, ProductType.Drink);

class ProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string[] Ingredients { get; set; }
    public double Price { get; set; }
    public ProductType ProductType { get; set; }

    public ProductModel(string name, string[] ingredients, double price, ProductType productType)
    {
        Name = name;
        Ingredients = ingredients;
        Price = price;
        Id = Guid.NewGuid();
        ProductType = productType;
    }
}

enum ProductType
{
    PizzaSmall,
    PizzaMedium,
    PizzaBig,
    Drink,
    Sauce,
    Snack
}

class PaymentModel
{
    public ProductModel[] Products { get; set; }
    public AddressModel Address { get; set; }
}

class AddressModel
{
    public string Address { get; set; }
}