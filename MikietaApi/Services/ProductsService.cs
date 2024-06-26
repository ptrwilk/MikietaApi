using Microsoft.EntityFrameworkCore;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Services;

public interface IProductsService
{
    ProductModel[] Get(AddressModel address);
    AdminProductModel[] GetAdminProducts(AddressModel address);
    AdminProductModel AddOrUpdateAdminProduct(AdminProductModel model, AddressModel address);
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
        var products = _context.Products.Include(x => x.Ingredients)
            .Include(x => x.Sizes)
            .Where(x => x.IsDeleted == false)
            .OrderBy(x => x.Index).ToList();

        return products.Select(x => Convert(x, address)).ToArray();
    }

    public AdminProductModel[] GetAdminProducts(AddressModel address)
    {
        var products = _context.Products.Include(x => x.Ingredients)
            .Include(x => x.Sizes)
            .Where(x => x.IsDeleted == false).OrderBy(x => x.Index).ToList();

        return products.Select(entity => new AdminProductModel
        {
            Id = entity.Id,
            ProductType = entity.ProductType,
            PizzaSizePrice = entity.Sizes.ToDictionary(x => x.Size, x => x.Price),
            Description = entity.Description,
            Name = entity.Name,
            Price = entity.Price ?? 0,
            ImageId = entity.ImageId,
            ImageUrl = ToImageUrl(entity, address),
            Ingredients = entity.Ingredients.Select(z => new IngredientModel
            {
                Id = z.Id,
                Name = z.Name
            }).ToArray()
        }).ToArray();
    }

    public AdminProductModel AddOrUpdateAdminProduct(AdminProductModel model, AddressModel address)
    {
        ProductEntity entity;
        if (model.Id is null)
        {
            entity = new ProductEntity
            {
                Ingredients = new List<IngredientEntity>(),
                Sizes = new List<PizzaSizeEntity>()
            };

            _context.Products.Add(entity);
            
            model.Id = entity.Id;
            
            
        }
        else
        {
            entity = _context.Products.Include(x => x.Ingredients).Include(x => x.Sizes).First(x => x.Id == model.Id);
        }

        var ingredients = _context.Ingredients.ToList();
        var ingredientIds = model.Ingredients.Select(x => x.Id).ToArray();

        entity.Name = model.Name;
        entity.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description;
        entity.Price = model.Price;
        entity.ProductType = model.ProductType;
        AddOrUpdateSizes(entity, model);
        entity.Ingredients.Clear();
        entity.Ingredients = ingredients.Where(x => ingredientIds.Any(z => z == x.Id)).ToList();
        entity.ImageId = model.ImageId;

        _context.SaveChanges();

        model.ImageUrl = ToImageUrl(entity, address);
        
        return model;
    }

    private void AddOrUpdateSizes(ProductEntity entity, AdminProductModel model)
    {
        if (model.ProductType == ProductType.Pizza)
        {
            if (entity.Sizes.Count == 0)
            {
                entity.Sizes.Add(new PizzaSizeEntity
                {
                    Size = PizzaType.Small
                });
                
                entity.Sizes.Add(new PizzaSizeEntity
                {
                    Size = PizzaType.Medium
                });
                
                entity.Sizes.Add(new PizzaSizeEntity
                {
                    Size = PizzaType.Large
                });
            }
            
            entity.Sizes.First(x => x.Size == PizzaType.Small).Price = model.PizzaSizePrice[PizzaType.Small];
            entity.Sizes.First(x => x.Size == PizzaType.Medium).Price = model.PizzaSizePrice[PizzaType.Medium];
            entity.Sizes.First(x => x.Size == PizzaType.Large).Price = model.PizzaSizePrice[PizzaType.Large];
        }
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
            Ingredients = entity.Ingredients.Select(Convert).ToArray(),
            Description = entity.Description,
            ProductType = entity.ProductType,
            Name = entity.Name,
            Price = entity.Price ?? entity.Sizes.First(x => x.Size == PizzaType.Small).Price,
            ImageUrl = ToImageUrl(entity, address),
            PizzaSizePrice = entity.Sizes.ToDictionary(x => x.Size, x => x.Price)
        };
    }

    private IngredientModel Convert(IngredientEntity entity)
    {
        return new IngredientModel
        {
            Id = entity.Id,
            Name = entity.Name,
            PriceSmall = entity.PriceSmall,
            PriceLarge = entity.PriceLarge,
            PriceMedium = entity.PriceMedium
        };
    }
    
    private string? ToImageUrl(ProductEntity entity, AddressModel address)
    {
        return entity.ImageId is not null ? $"{address}/image/{entity.ImageId}" : null;
    }
}