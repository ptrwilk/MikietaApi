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

            AddMargherita(17, PizzaType.Small);
            AddCipolla(18, PizzaType.Small);
            AddFunghi(18, PizzaType.Small);
            AddSalami(18, PizzaType.Small);
            AddFunghiEProsciutto(18, PizzaType.Small);
            AddMargherita(22, PizzaType.Medium);
            AddCipolla(23, PizzaType.Medium);
            AddFunghi(23, PizzaType.Medium);
            AddSalami(26, PizzaType.Medium);
            AddFunghiEProsciutto(26, PizzaType.Medium);
            AddMargherita(33, PizzaType.Large);
            AddCipolla(34, PizzaType.Large);
            AddFunghi(34, PizzaType.Large);
            AddSalami(36, PizzaType.Large);
            AddFunghiEProsciutto(36, PizzaType.Large);

            AddProduct("Czosnkowy", "firmowy sos XXX idealnie komponujący się ze smakiem każdej naszej pizzy",
                4, ProductType.Sauce);
            AddProduct("Czosnkowo-szczypiorkowy", "firmowy sos XXX, ketchup", 4, ProductType.Sauce);
            AddProduct("Pomidorowy łagodny", null, 4, ProductType.Sauce);

            _context.SaveChanges();
        }

        if (!_context.Settings.Any())
        {
            foreach (var key in SettingEntity.Keys)
            {
                var value = key.Contains("Open") || key.Contains("Delivery") ? "00:00" : null;
                _context.Settings.Add(new SettingEntity
                {
                    Key = key,
                    Value = value
                });
            }
        
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

    private void AddPizza(string name, string[] ingredients, double price, PizzaType type)
    {
        var ingredient = _context.Ingredients.Where(x => ingredients.Any(i => i == x.Name)).ToArray();
        _context.Products.Add(new ProductEntity
        {
            Name = name,
            ProductType = ProductType.Pizza,
            PizzaType = type,
            Ingredients = ingredient,
            Price = price,
        });          
    }

    private void AddMargherita(double price, PizzaType type) =>
        AddPizza("Margherita", new[] { "Ser", "Sos pomidorowy" }, price, type);
    
    private void AddFunghi(double price, PizzaType type) =>
        AddPizza("Funghi", new[] { "Ser", "Sos pomidorowy", "Pieczarki" }, price, type);
    
    private void AddFunghiEProsciutto(double price, PizzaType type) =>
        AddPizza("FunghiEProsciutto", new[] { "Ser", "Sos pomidorowy", "Szynka", "Pieczarki" }, price, type);
    
    private void AddCipolla(double price, PizzaType type) =>
        AddPizza("Cipolla", new[] { "Ser", "Sos pomidorowy", "Cebula" }, price, type);
    
    private void AddSalami(double price, PizzaType type) =>
        AddPizza("Salami", new[] { "Ser", "Sos pomidorowy", "Salami" }, price, type);
}