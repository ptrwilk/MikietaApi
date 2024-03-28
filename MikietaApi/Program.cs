using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddPolicy("MyPolicy",
        b => { b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }
    )
);

var app = builder.Build();

app.UseCors("MyPolicy");

app.MapGet("/", () => new[] { "a", "b", "c" });


app.MapGet("/pizza", (string size) =>
{
    if (size == "small")
    {
        return Results.Ok(new[] { Margherita(17), Cipolla(18), Funghi(18), Salami(18), FunghiEProsciutto(18) });
    }

    if (size == "medium")
    {
        return Results.Ok(new[] { Margherita(22), Cipolla(23), Funghi(23), Salami(26), FunghiEProsciutto(26) });
    }

    if (size == "big")
    {
        return Results.Ok(new[] { Margherita(33), Cipolla(34), Funghi(34), Salami(36), FunghiEProsciutto(36) });
    }

    return Results.NotFound();
});

app.MapGet("/drink", (string type) =>
{
    if (type == "beer")
    {
        return Results.Ok(new[] { CreateDrink("Kozel Lezak", 10), CreateDrink("Żywiec Klasyk", 11) });
    }

    if (type == "mulled-wine")
    {
        return Results.Ok(new[]
        {
            CreateDrink("Grzaniec XXL 0.5L", 19), CreateDrink("Grzaniec Duży", 14),
            CreateDrink("Grzaniec Mały", 10)
        });
    }

    if (type == "other")
    {
        return Results.Ok(new[]
        {
            CreateDrink("Wino Czerwone", 15, new[] { "Kieliszek" }),
            CreateDrink("Wino Białe", 15, new[] { "Kieliszek" }), CreateDrink("Kawa Czarna", 8),
            CreateDrink("Herbata", 5, new[] { "Filiżanka" }), CreateDrink("Herbata", 9, new[] { "Dzbanek" })
        });
    }

    return Results.NotFound();
});

app.MapGet("/snack", () =>
{
    return Results.Ok(new[]
    {
        //TODO: Extend filter to support this case
        new ProductModel("CALZONE pieróg z nadzieniem", new []{"w każdym pierożku, ser, sos czosnkowy + 4 dowolne składniki do wyboru"}, 27),
        new ProductModel("Frytki Cieńkie Małe", new []{"Porcja chrupiących frytek, ketchup"}, 8),
        new ProductModel("Frytki Cieńkie Duże", new []{"Porcja chrupiących frytek, ketchup"}, 10),
        new ProductModel("Frytki Belgijskie Małe", new []{"Porcja chrupiących frytek, ketchup"}, 8),
        new ProductModel("Frytki Belgijskie Duże", new []{"Porcja chrupiących frytek, ketchup"}, 10)
    });
});

app.MapGet("/sauce", () =>
{
    return Results.Ok(new[]
    {
        //TODO: Extend filter to support this case
        new ProductModel("Czosnkowy", new []{"firmowy sos XXX idealnie komponujący się ze smakiem każdej naszej pizzy"}, 4),
        new ProductModel("Czosnkowo-szczypiorkowy", new []{"firmowy sos XXX, ketchup"}, 4),
        new ProductModel("Pomidorowy łagodny", new []{""}, 4)
    });
});

app.MapPost("/payment", (PaymentModel model) => Results.Ok(Guid.NewGuid()));

app.MapGet("/delivery/{deliveryId}", (Guid deliveryId) =>
{
    return Results.Ok($"Test {deliveryId}");
});

app.Run();

static ProductModel Margherita(double price) => new ProductModel("Margherita", new[] { "ser", "sos pomidorowy" }, price);

static ProductModel Cipolla(double price) =>
    new ProductModel("Cipolla", new[] { "ser", "sos pomidorowy", "cebula" }, price);

static ProductModel Funghi(double price) =>
    new ProductModel("Funghi", new[] { "ser", "sos pomidorowy", "pieczarki" }, price);

static ProductModel Salami(double price) => new ProductModel("Salami", new[] { "ser", "sos pomidorowy", "salami" }, price);

static ProductModel FunghiEProsciutto(double price) => new ProductModel("Funghi e Prosciutto",
    new[] { "ser", "sos pomidorowy", "szynka", "pieczarki" }, price);

ProductModel CreateDrink(string name, double price, string[]? ingredients = null) =>
    new ProductModel(name, ingredients ?? Array.Empty<string>(), price);

class ProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string[] Ingredients { get; set; }
    public double Price { get; set; }

    public ProductModel(string name, string[] ingredients, double price)
    {
        Name = name;
        Ingredients = ingredients;
        Price = price;
        Id = Guid.NewGuid();
    }
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