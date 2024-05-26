using MikietaApi.Services;

namespace MikietaApi.Routes;

public static class IngredientRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapGet("ingredient", Get);

        return app;
    }
    
    private static IResult Get(IIngredientService service)
    {
        return Results.Ok(service.Get());
    }
}