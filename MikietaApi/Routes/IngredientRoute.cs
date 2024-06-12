using Microsoft.AspNetCore.Authorization;
using MikietaApi.Models;
using MikietaApi.Services;

namespace MikietaApi.Routes;

public static class IngredientRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapGet("ingredient", Get);
        app.MapPut("ingredient", Update);
        app.MapDelete("ingredient/{ingredientId}", Delete);

        return app;
    }
    
    [Authorize]
    private static IResult Get(IIngredientService service)
    {
        return Results.Ok(service.Get());
    }
    
    [Authorize]
    private static IResult Update(IIngredientService service, IngredientModel model)
    {
        return Results.Ok(service.Update(model));
    }
    
    [Authorize]
    private static IResult Delete(IIngredientService service, Guid ingredientId)
    {
        return Results.Ok(service.Delete(ingredientId));
    }
}