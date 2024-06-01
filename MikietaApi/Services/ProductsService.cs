using Microsoft.EntityFrameworkCore;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Services;

public interface IProductsService
{
    ProductModel[] Get();
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

    private ProductModel Convert(ProductEntity entity)
    {
        return new ProductModel
        {
            Id = entity.Id,
            Ingredientss = null,//TODO
            Ingredients = entity.Description != null ? new []{ entity.Description} : entity.Ingredients.Select(x => x.Name).ToArray(),
            ProductType = (ProductType)Enum.Parse(typeof(ProductType), entity.ProductType.ToString()),
            Name = entity.Name,
            Price = entity.Price,
            Quantity = 0
        };
    }
}