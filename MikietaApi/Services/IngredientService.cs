using MikietaApi.Data;
using MikietaApi.Models;

namespace MikietaApi.Services;

public interface IIngredientService
{
    IngredientModel[] Get();
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
        return _context.Ingredients.ToList().Select(x => new IngredientModel
        {
            Id = x.Id,
            Name = x.Name
        }).ToArray();
    }
}