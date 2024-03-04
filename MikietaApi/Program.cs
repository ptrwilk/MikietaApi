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
        return Results.Ok(new[] { new DrinkModel("Kozel Lezak", 10), new DrinkModel("Żywiec Klasyk", 11) });
    }

    if (type == "mulled-wine")
    {
        return Results.Ok(new[]
        {
            new DrinkModel("Grzaniec XXL 0.5L", 19), new DrinkModel("Grzaniec Duży", 14),
            new DrinkModel("Grzaniec Mały", 10)
        });
    }

    if (type == "other")
    {
        return Results.Ok(new[]
        {
            new DrinkModel("Wino Czerwone", 15, new[] { "Kieliszek" }),
            new DrinkModel("Wino Białe", 15, new[] { "Kieliszek" }), new DrinkModel("Kawa Czarna", 8),
            new DrinkModel("Herbata", 5, new[] { "Filiżanka" }), new DrinkModel("Herbata", 9, new[] { "Dzbanek" })
        });
    }

    return Results.NotFound();
});

app.MapGet("/snack", (string type) =>
{
    return Results.Ok(new[]
    {
        //TODO: Extend filter to support this case
        new PizzaModel("CALZONE pieróg z nadzieniem", new []{"w każdym pierożku, ser, sos czosnkowy + 4 dowolne składniki do wyboru"}, 27),
        new PizzaModel("Frytki Cieńkie Małe", new []{"Porcja chrupiących frytek, ketchup"}, 8),
        new PizzaModel("Frytki Cieńkie Duże", new []{"Porcja chrupiących frytek, ketchup"}, 10),
        new PizzaModel("Frytki Belgijskie Małe", new []{"Porcja chrupiących frytek, ketchup"}, 8),
        new PizzaModel("Frytki Belgijskie Duże", new []{"Porcja chrupiących frytek, ketchup"}, 10)
    });
});

app.MapGet("/sauce", (string type) =>
{
    return Results.Ok(new[]
    {
        //TODO: Extend filter to support this case
        new PizzaModel("Czosnkowy", new []{"firmowy sos XXX idealnie komponujący się ze smakiem każdej naszej pizzy"}, 4),
        new PizzaModel("Czosnkowo-szczypiorkowy", new []{"firmowy sos XXX, ketchup"}, 4),
        new PizzaModel("Pomidorowy łagodny", new []{""}, 4)
    });
});

app.Run();

static PizzaModel Margherita(double price) => new PizzaModel("Margherita", new[] { "ser", "sos pomidorowy" }, price);

static PizzaModel Cipolla(double price) =>
    new PizzaModel("Cipolla", new[] { "ser", "sos pomidorowy", "cebula" }, price);

static PizzaModel Funghi(double price) =>
    new PizzaModel("Funghi", new[] { "ser", "sos pomidorowy", "pieczarki" }, price);

static PizzaModel Salami(double price) => new PizzaModel("Salami", new[] { "ser", "sos pomidorowy", "salami" }, price);

static PizzaModel FunghiEProsciutto(double price) => new PizzaModel("Funghi e Prosciutto",
    new[] { "ser", "sos pomidorowy", "szynka", "pieczarki" }, price);

record PizzaModel(string Name, string[] Ingredients, double Price);

record DrinkModel(string Name, double Price, string[]? Ingredients = null);