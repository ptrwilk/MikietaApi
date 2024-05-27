using MikietaApi.Models;
using MikietaApi.Services;

namespace MikietaApi.Routes;

//TODO: Rename Products to Product (Route, Service)
public static class ProductsRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapGet("menu", GetProducts);
        app.MapGet("products", GetAdminProducts);
        app.MapPut("products", UpdateAdminProduct);
        app.MapDelete("products/{productId}", Delete);

        return app;
    }

    private static IResult GetProducts(IProductsService service)
    {
        return Results.Ok(service.Get());
    }
    
    private static IResult GetAdminProducts(IProductsService service)
    {
        return Results.Ok(service.GetAdminProducts());
    }
    
    private static IResult UpdateAdminProduct(IProductsService service, AdminProductModel2 model)
    {
        return Results.Ok(service.AddOrUpdateAdminProduct(model));
    }
    
    private static IResult Delete(IProductsService service, Guid productId)
    {
        return Results.Ok(service.Delete(productId));
    }
}