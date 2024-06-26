using Microsoft.EntityFrameworkCore;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi;

public class DbSeeder
{
    private readonly DataContext _context;

    public DbSeeder(DataContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        _context.Database.Migrate();

        if (!_context.Ingredients.Any())
        {
            _context.Ingredients.Add(new IngredientEntity { Name = "Ser" });
            _context.Ingredients.Add(new IngredientEntity { Name = "Sos pomidorowy" });
            _context.Ingredients.Add(new IngredientEntity { Name = "Cebula" });
            _context.Ingredients.Add(new IngredientEntity { Name = "Pieczarki" });
            _context.Ingredients.Add(new IngredientEntity { Name = "Salami" });
            _context.Ingredients.Add(new IngredientEntity { Name = "Szynka" });

            _context.SaveChanges();

            AddDrink("Kozel Lezak", 10);
            AddDrink("Żywiec Klasyk", 11);
            AddDrink("Grzaniec XXL 0.5L", 19);
            AddDrink("Grzaniec Duży", 14);
            AddDrink("Grzaniec Mały", 10);

            AddProduct("CALZONE pieróg z nadzieniem",
                "w każdym pierożku, ser, sos czosnkowy + 4 dowolne składniki do wyboru", 27,
                ProductType.Snack);
            AddProduct("Frytki Cieńkie Małe", "Porcja chrupiących frytek, ketchup", 8, ProductType.Snack);
            AddProduct("Frytki Cieńkie Duże", "Porcja chrupiących frytek, ketchup", 10, ProductType.Snack);
            AddProduct("Frytki Belgijskie Małe", "Porcja chrupiących frytek, ketchup", 8,
                ProductType.Snack);
            AddProduct("Frytki Belgijskie Duże", "Porcja chrupiących frytek, ketchup", 10,
                ProductType.Snack);

            AddMargherita(new[] { 17d, 22d, 33d });
            AddCipolla(new[] { 18d, 23d, 34d });
            AddFunghi(new[] { 18d, 23d, 34d });
            AddFunghi(new[] { 18d, 26d, 36d });
            AddFunghiEProsciutto(new[] { 18d, 26d, 36d });

            AddProduct("Czosnkowy", "firmowy sos XXX idealnie komponujący się ze smakiem każdej naszej pizzy",
                4, ProductType.Sauce);
            AddProduct("Czosnkowo-szczypiorkowy", "firmowy sos XXX, ketchup", 4, ProductType.Sauce);
            AddProduct("Pomidorowy łagodny", null, 4, ProductType.Sauce);

            _context.SaveChanges();
        }

        var keyAdded = false;
        foreach (var key in SettingEntity.Keys)
        {
            if (_context.Settings.All(x => x.Key != key))
            {
                string? value = null;
                if (SettingEntity.Times.Any(z => z.Equals(key)))
                {
                    value = "00:00:00";
                }

                _context.Settings.Add(new SettingEntity
                {
                    Key = key,
                    Value = value
                });
                keyAdded = true;
            }
        }

        if (keyAdded)
        {
            _context.SaveChanges();
        }
    }

    private void AddDrink(string name, double price)
    {
        _context.Products.Add(new ProductEntity
        {
            Name = name,
            ProductType = ProductType.Drink,
            Price = price,
        });
    }

    private void AddProduct(string name, string? description, double price, ProductType type)
    {
        _context.Products.Add(new ProductEntity
        {
            Name = name,
            ProductType = type,
            Description = description,
            Price = price,
        });
    }

    private void AddPizza(string name, string[] ingredients, double[] prices)
    {
        var ingredient = _context.Ingredients.Where(x => ingredients.Any(i => i == x.Name)).ToArray();
        _context.Products.Add(new ProductEntity
        {
            Name = name,
            ProductType = ProductType.Pizza,
            Ingredients = ingredient,
            Sizes = prices.Select((x, i) => new PizzaSizeEntity
            {
                Price = x,
                Size = i switch
                {
                    0 => PizzaType.Small,
                    1 => PizzaType.Medium,
                    _ => PizzaType.Large
                }
            }).ToArray()
        });
    }

    private void AddMargherita(double[] prices) =>
        AddPizza("Margherita", new[] { "Ser", "Sos pomidorowy" }, prices);

    private void AddFunghi(double[] prices) =>
        AddPizza("Funghi", new[] { "Ser", "Sos pomidorowy", "Pieczarki" }, prices);

    private void AddFunghiEProsciutto(double[] prices) =>
        AddPizza("FunghiEProsciutto", new[] { "Ser", "Sos pomidorowy", "Szynka", "Pieczarki" }, prices);

    private void AddCipolla(double[] prices) =>
        AddPizza("Cipolla", new[] { "Ser", "Sos pomidorowy", "Cebula" }, prices);

    private void AddSalami(double[] prices) =>
        AddPizza("Salami", new[] { "Ser", "Sos pomidorowy", "Salami" }, prices);
}