using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;

namespace MikietaApi.Services;

public interface IIngredientService
{
    IngredientModel[] Get();
    IngredientModel Update(IngredientModel model);
    bool Delete(Guid ingredientId);
}

public class IngredientService : IIngredientService
{
    private readonly DataContext _context;

    public IngredientService(DataContext context)
    {
        _context = context;
    }

    public IngredientModel[] Get()
    {
        return _context.Ingredients.Where(x => x.IsDeleted == false)
            .OrderBy(x => x.Index).ToList()
            .Select(x => new IngredientModel
            {
                Id = x.Id,
                Name = x.Name,
                PriceSmall = x.PriceSmall,
                PriceLarge = x.PriceLarge,
                PriceMedium = x.PriceMedium
            }).ToArray();
    }
    
    public IngredientModel Update(IngredientModel model)
    {
        IngredientEntity entity;
        if (model.Id is null)
        {
            entity = new IngredientEntity();

            _context.Ingredients.Add(entity);

            model.Id = entity.Id;
        }
        else
        {
            entity = _context.Ingredients.First(x => x.Id == model.Id);
        }

        entity.Name = model.Name ?? "";
        entity.PriceSmall = model.PriceSmall;
        entity.PriceMedium = model.PriceMedium;
        entity.PriceLarge = model.PriceLarge;

        _context.SaveChanges();
        
        return model;
    }

    public bool Delete(Guid ingredientId)
    {
        var product = _context.Ingredients.First(x => x.Id == ingredientId);

        product.IsDeleted = true;

        _context.SaveChanges();

        return true;
    }
}