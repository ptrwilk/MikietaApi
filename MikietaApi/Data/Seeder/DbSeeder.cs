using Microsoft.EntityFrameworkCore;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Data;

public class DbSeeder
{
    private readonly DataContext _context;
    private readonly ProductsParser _productsParser;

    public DbSeeder(DataContext context, ProductsParser productsParser)
    {
        _context = context;
        _productsParser = productsParser;
    }

    public void Seed()
    {
        if (!_context.IsInMemoryProvider())
        {
            _context.Database.Migrate();
        }

        if (!_context.Ingredients.Any())
        {
            var products = _productsParser.Parse();

            var ingredients = products.Where(x => x.Ingredients is not null).SelectMany(x => x.Ingredients!).Distinct();

            foreach (var ingredient in ingredients)
            {
                _context.Ingredients.Add(new IngredientEntity { Name = ingredient });
            }

            _context.SaveChanges();

            var index = 1;
            foreach (var drink in products.Where(x => x.ProductType == ProductType.Drink))
            {
                AddDrink(drink.Name, drink.Price!.Value, index);
                index++;
            }
            
            foreach (var product in products.Where(x => x.ProductType != ProductType.Drink && x.ProductType != ProductType.Pizza))
            {
                AddProduct(product.Name, product.Description, product.Price!.Value, product.ProductType, index);
                index++;
            }
            
            foreach (var pizza in products.Where(x => x.ProductType == ProductType.Pizza))
            {
                AddPizza(pizza.Name, pizza.Ingredients!, new []{ pizza.MediumPrice, pizza.LargePrice }, index);
                index++;
            }

            AddProduct("SOS POMIDOROWY", null,
                4, ProductType.Sauce, ++index);
            AddProduct("SOS CZOSNKOWY", null,
                4, ProductType.Sauce, ++index);
            AddProduct("SOS PIKANTNY", null,
                4, ProductType.Sauce, ++index);
            AddProduct("SOS BARBECUE", null,
                4, ProductType.Sauce, ++index);
            AddProduct("KETCHUP", null,
                4, ProductType.Sauce, ++index);
            AddProduct("OLIWA", null,
                4, ProductType.Sauce, ++index);
            AddProduct("DODATKI MIESO/ SER DUŻA PIZZA", null,
                7, ProductType.Sauce, ++index);
            AddProduct("DODATKI WARZYWNE DUŻA PIZZA", null,
                5, ProductType.Sauce, ++index);
            AddProduct("DODATKI MIESO/ SER ŚREDNIA PIZZA", null,
                6, ProductType.Sauce, ++index);
            AddProduct("DODATKI WARZYWNE ŚREDNIA PIZZA", null,
                4, ProductType.Sauce, ++index);

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

    private void AddDrink(string name, decimal price, int index)
    {
        _context.Products.Add(new ProductEntity
        {
            Name = name,
            ProductType = ProductType.Drink,
            Price = price,
            Index = index
        });
    }

    private void AddProduct(string name, string? description, decimal price, ProductType type, int index)
    {
        _context.Products.Add(new ProductEntity
        {
            Name = name,
            ProductType = type,
            Description = description,
            Price = price,
            Index = index
        });
    }

    private void AddPizza(string name, string[] ingredients, decimal?[] prices, int index)
    {
        var ingredient = _context.Ingredients.Where(x => ingredients.Any(i => i == x.Name)).ToArray();
        _context.Products.Add(new ProductEntity
        {
            Name = name,
            ProductType = ProductType.Pizza,
            Ingredients = ingredient,
            Index = index,
            Sizes = prices.Select((x, i) => new PizzaSizeEntity
            {
                Price = x ?? 0,
                Size = i switch
                {
                    0 => PizzaType.Medium,
                    _ => PizzaType.Large
                }
            }).ToArray()
        });
    }
}