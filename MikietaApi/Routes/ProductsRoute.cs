using MikietaApi.Services;

namespace MikietaApi.Routes;

public static class ProductsRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapGet("menu", GetProducts);
        
        return app;
    }
    
    private static IResult GetProducts(IProductsService service)
    {
        return Results.Ok(service.Get());
    }
}