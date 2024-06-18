using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using MikietaApi.Models;
using MikietaApi.Services;

namespace MikietaApi.Routes;

public static class OrderRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapPost("order", Order);
        app.MapGet("order/success", Success);
        app.MapGet("order/cancel", Cancel);
        app.MapGet("order", GetAll);
        app.MapGet("order/{orderId}/products", Get);
        app.MapGet("order/{orderId}", GetSingle);
        app.MapGet("order/{orderId}/status", GetStatus);
        app.MapPut("order", Update);
        app.MapPut("order/{orderId}/product", UpdateProduct);
        app.MapPut("order/{orderId}/clear-can-clear-basket", ClearCanClearBasket);

        return app;
    }

    private static IResult Order(IOrderService service, IValidator<OrderModel> validator, OrderModel model)
    {
        var validation = validator.Validate(model);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }

        try
        {
            return Results.Ok(service.Order(model));
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static IResult Success(IOrderService service, ConfigurationOptions options, string sessionId)
    {
        try
        {
            var id = service.OrderSuccess(sessionId);
            return Results.Redirect($"{options.WebsiteUrl}/zamowienie/{id}");
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static IResult Cancel(IOrderService service, ConfigurationOptions options)
    {
        try
        {
            service.OrderCanceled();
            return Results.Redirect($"{options.WebsiteUrl}/kasa");
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [Authorize]
    private static IResult GetAll(IOrderService service)
    {
        return Results.Ok(service.GetAll());
    }

    [Authorize]
    private static IResult Get(IOrderService service, Guid orderId)
    {
        return Results.Ok(service.Get(orderId));
    }

    [Authorize]
    private static IResult Update(IOrderService service, AdminOrderModel model)
    {
        return Results.Ok(service.Update(model));
    }

    [Authorize]
    private static IResult UpdateProduct(IOrderService service, Guid orderId, AdminOrderedProductModel model)
    {
        return Results.Ok(service.UpdateProduct(orderId, model));
    }

    [Authorize]
    private static IResult GetSingle(IOrderService service, Guid orderId)
    {
        return Results.Ok(service.GetSingle(orderId));
    }
    
    private static IResult GetStatus(IOrderService service, Guid orderId)
    {
        return Results.Ok(service.GetStatus(orderId));
    }
    
    private static IResult ClearCanClearBasket(IOrderService service, Guid orderId)
    {
        return Results.Ok(service.ClearCanClearBasket(orderId));
    }
}