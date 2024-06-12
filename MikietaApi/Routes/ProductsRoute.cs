using Microsoft.AspNetCore.Authorization;
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
        app.MapPut("products", AddOrUpdateAdminProduct);
        app.MapDelete("products/{productId}", Delete);

        return app;
    }

    private static IResult GetProducts(IProductsService service, HttpContext context)
    {
        return Results.Ok(service.Get(new AddressModel(context)));
    }
    
    [Authorize]
    private static IResult GetAdminProducts(IProductsService service, HttpContext context)
    {
        return Results.Ok(service.GetAdminProducts(new AddressModel(context)));
    }
    
    [Authorize]
    private static IResult AddOrUpdateAdminProduct(IProductsService service, HttpContext context, AdminProductModel2 model)
    {
        return Results.Ok(service.AddOrUpdateAdminProduct(model, new AddressModel(context)));
    }
    
    [Authorize]
    private static IResult Delete(IProductsService service, Guid productId)
    {
        return Results.Ok(service.Delete(productId));
    }
}