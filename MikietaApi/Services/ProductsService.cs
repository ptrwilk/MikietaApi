using Microsoft.EntityFrameworkCore;
using MikietaApi.Converters;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Services;

public interface IProductsService
{
    ProductModel[] Get();
    AdminProductModel2[] GetAdminProducts();
    AdminProductModel2 UpdateAdminProduct(AdminProductModel2 model);
}

public class ProductsService : IProductsService
{
    private readonly DataContext _context;

    public ProductsService(DataContext context)
    {
        _context = context;
    }
    
    public ProductModel[] Get()
    {
        var products = _context.Products.Include(x => x.Ingredients).ToList();

        return products.Select(Convert).ToArray();
    }

    public AdminProductModel2[] GetAdminProducts()
    {
        var products = _context.Products.Include(x => x.Ingredients).ToList();

        return products.Select(entity => new AdminProductModel2
        {
            Id = entity.Id,
            ProductType = EnumConverter.Convert(entity.ProductType),
            Description = entity.Description,
            Name = entity.Name,
            Price = entity.Price,
            Ingredients = entity.Ingredients.Select(z => new IngredientModel
            {
                Id = z.Id,
                Name = z.Name
            }).ToArray()
        }).ToArray();
    }

    public AdminProductModel2 UpdateAdminProduct(AdminProductModel2 model)
    {
        var product = _context.Products.Include(x => x.Ingredients).First(x => x.Id == model.Id);

        var ingredients = _context.Ingredients.ToList();
        var ingredientIds = model.Ingredients.Select(x => x.Id).ToArray();

        product.Name = model.Name;
        product.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description;
        product.Price = model.Price;
        product.ProductType = EnumConverter.Convert(model.ProductType);
        product.Ingredients.Clear();
        product.Ingredients = ingredients.Where(x => ingredientIds.Any(z => z == x.Id)).ToList();

        _context.SaveChanges();
        
        return model;
    }

    private ProductModel Convert(ProductEntity entity)
    {
        return new ProductModel
        {
            Id = entity.Id,
            Ingredients = entity.Description != null ? new []{ entity.Description} : entity.Ingredients.Select(x => x.Name).ToArray(),
            ProductType = EnumConverter.Convert(entity.ProductType),
            Name = entity.Name,
            Price = entity.Price
        };
    }
}