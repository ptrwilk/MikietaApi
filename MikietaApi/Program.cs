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

app.Run();

static PizzaModel Margherita(double price) => new PizzaModel("Margherita", new[] { "ser", "sos pomidorowy" }, price);
static PizzaModel Cipolla(double price) => new PizzaModel("Cipolla", new[] { "ser", "sos pomidorowy", "cebula" }, price);
static PizzaModel Funghi(double price) => new PizzaModel("Funghi", new[] { "ser", "sos pomidorowy", "pieczarki" }, price);
static PizzaModel Salami(double price) => new PizzaModel("Salami", new[] { "ser", "sos pomidorowy", "salami" }, price);
static PizzaModel FunghiEProsciutto(double price) => new PizzaModel("Funghi e Prosciutto", new[] { "ser", "sos pomidorowy", "szynka", "pieczarki" }, price);

record PizzaModel(string Name, string[] Ingredients, double Price);