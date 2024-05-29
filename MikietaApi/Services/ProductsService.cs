using Microsoft.EntityFrameworkCore;
using MikietaApi.Converters;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Services;

public interface IProductsService
{
    ProductModel[] Get(AddressModel address);
    AdminProductModel2[] GetAdminProducts(AddressModel address);
    AdminProductModel2 AddOrUpdateAdminProduct(AdminProductModel2 model, AddressModel address);
    bool Delete(Guid productId);
}

public class ProductsService : IProductsService
{
    private readonly DataContext _context;
    private readonly ConfigurationOptions _options;

    public ProductsService(DataContext context, ConfigurationOptions options)
    {
        _context = context;
        _options = options;
    }
    
    public ProductModel[] Get(AddressModel address)
    {
        var products = _context.Products.Include(x => x.Ingredients).ToList();

        return products.Select(x => Convert(x, address)).ToArray();
    }

    public AdminProductModel2[] GetAdminProducts(AddressModel address)
    {
        var products = _context.Products.Include(x => x.Ingredients)
            .Where(x => x.IsDeleted == false).ToList();

        return products.Select(entity => new AdminProductModel2
        {
            Id = entity.Id,
            ProductType = EnumConverter.Convert(entity.ProductType),
            Description = entity.Description,
            Name = entity.Name,
            Price = entity.Price,
            ImageId = entity.ImageId,
            ImageUrl = ToImageUrl(entity, address),
            Ingredients = entity.Ingredients.Select(z => new IngredientModel
            {
                Id = z.Id,
                Name = z.Name
            }).ToArray()
        }).ToArray();
    }

    public AdminProductModel2 AddOrUpdateAdminProduct(AdminProductModel2 model, AddressModel address)
    {
        ProductEntity entity;
        if (model.Id is null)
        {
            entity = new ProductEntity
            {
                Ingredients = new List<IngredientEntity>()
            };

            _context.Products.Add(entity);
            
            model.Id = entity.Id;
        }
        else
        {
            entity = _context.Products.Include(x => x.Ingredients).First(x => x.Id == model.Id);
        }

        var ingredients = _context.Ingredients.ToList();
        var ingredientIds = model.Ingredients.Select(x => x.Id).ToArray();

        entity.Name = model.Name;
        entity.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description;
        entity.Price = model.Price;
        entity.ProductType = EnumConverter.Convert(model.ProductType);
        entity.Ingredients.Clear();
        entity.Ingredients = ingredients.Where(x => ingredientIds.Any(z => z == x.Id)).ToList();
        entity.ImageId = model.ImageId;

        _context.SaveChanges();

        model.ImageUrl = ToImageUrl(entity, address);
        
        return model;
    }

    public bool Delete(Guid productId)
    {
        var product = _context.Products.First(x => x.Id == productId);

        product.IsDeleted = true;

        _context.SaveChanges();

        return true;
    }

    private ProductModel Convert(ProductEntity entity, AddressModel address)
    {
        return new ProductModel
        {
            Id = entity.Id,
            Ingredients = entity.Description != null ? new []{ entity.Description} : entity.Ingredients.Select(x => x.Name).ToArray(),
            ProductType = EnumConverter.Convert(entity.ProductType),
            Name = entity.Name,
            Price = entity.Price,
            ImageUrl = ToImageUrl(entity, address)
        };
    }
    
    private string? ToImageUrl(ProductEntity entity, AddressModel address)
    {
        return entity.ImageId is not null ? $"{address}/image/{entity.ImageId}" : null;
    }
}