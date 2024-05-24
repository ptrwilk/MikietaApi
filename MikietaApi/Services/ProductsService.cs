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
        var products = _context.Products.ToList();

        return products.Select(entity => new AdminProductModel2
        {
            Id = entity.Id,
            ProductType = EnumConverter.Convert(entity.ProductType),
            Description = entity.Description,
            Name = entity.Name,
            Price = entity.Price
        }).ToArray();
    }

    public AdminProductModel2 UpdateAdminProduct(AdminProductModel2 model)
    {
        var product = _context.Products.First(x => x.Id == model.Id);

        product.Name = model.Name;
        product.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description;
        product.Price = model.Price;
        product.ProductType = EnumConverter.Convert(model.ProductType);

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