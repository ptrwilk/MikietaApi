using Microsoft.EntityFrameworkCore;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Data.Entities.Enums;

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
                ProductTypeEntity.Snack);
            AddProduct("Frytki Cieńkie Małe", "Porcja chrupiących frytek, ketchup", 8, ProductTypeEntity.Snack);
            AddProduct("Frytki Cieńkie Duże", "Porcja chrupiących frytek, ketchup", 10, ProductTypeEntity.Snack);
            AddProduct("Frytki Belgijskie Małe", "Porcja chrupiących frytek, ketchup", 8,
                ProductTypeEntity.Snack);
            AddProduct("Frytki Belgijskie Duże", "Porcja chrupiących frytek, ketchup", 10,
                ProductTypeEntity.Snack);

            AddMargherita(17, ProductTypeEntity.Pizza);
            AddCipolla(18, ProductTypeEntity.Pizza);
            AddFunghi(18, ProductTypeEntity.Pizza);
            AddSalami(18, ProductTypeEntity.Pizza);
            AddFunghiEProsciutto(18, ProductTypeEntity.Pizza);
            AddMargherita(22, ProductTypeEntity.Pizza);
            AddCipolla(23, ProductTypeEntity.Pizza);
            AddFunghi(23, ProductTypeEntity.Pizza);
            AddSalami(26, ProductTypeEntity.Pizza);
            AddFunghiEProsciutto(26, ProductTypeEntity.Pizza);
            AddMargherita(33, ProductTypeEntity.Pizza);
            AddCipolla(34, ProductTypeEntity.Pizza);
            AddFunghi(34, ProductTypeEntity.Pizza);
            AddSalami(36, ProductTypeEntity.Pizza);
            AddFunghiEProsciutto(36, ProductTypeEntity.Pizza);

            AddProduct("Czosnkowy", "firmowy sos XXX idealnie komponujący się ze smakiem każdej naszej pizzy",
                4, ProductTypeEntity.Sauce);
            AddProduct("Czosnkowo-szczypiorkowy", "firmowy sos XXX, ketchup", 4, ProductTypeEntity.Sauce);
            AddProduct("Pomidorowy łagodny", null, 4, ProductTypeEntity.Sauce);

            _context.SaveChanges();
        }
    }

    private void AddDrink(string name, double price)
    {
        _context.Products.Add(new ProductEntity
        {
            Name = name,
            ProductType = ProductTypeEntity.Drink,
            Price = price,
        });
    }

    private void AddProduct(string name, string? description, double price, ProductTypeEntity type)
    {
        _context.Products.Add(new ProductEntity
        {
            Name = name,
            ProductType = type,
            Description = description,
            Price = price,
        });        
    }

    private void AddPizza(string name, string[] ingredients, double price, ProductTypeEntity type)
    {
        var ingredient = _context.Ingredients.Where(x => ingredients.Any(i => i == x.Name)).ToArray();
        _context.Products.Add(new ProductEntity
        {
            Name = name,
            ProductType = type,
            Ingredients = ingredient,
            Price = price,
        });          
    }

    private void AddMargherita(double price, ProductTypeEntity type) =>
        AddPizza("Margherita", new[] { "Ser", "Sos pomidorowy" }, price, type);
    
    private void AddFunghi(double price, ProductTypeEntity type) =>
        AddPizza("Funghi", new[] { "Ser", "Sos pomidorowy", "Pieczarki" }, price, type);
    
    private void AddFunghiEProsciutto(double price, ProductTypeEntity type) =>
        AddPizza("FunghiEProsciutto", new[] { "Ser", "Sos pomidorowy", "Szynka", "Pieczarki" }, price, type);
    
    private void AddCipolla(double price, ProductTypeEntity type) =>
        AddPizza("Cipolla", new[] { "Ser", "Sos pomidorowy", "Cebula" }, price, type);
    
    private void AddSalami(double price, ProductTypeEntity type) =>
        AddPizza("Salami", new[] { "Ser", "Sos pomidorowy", "Salami" }, price, type);
}