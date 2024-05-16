using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using MikietaApi.Hubs;
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

        return app;
    }

    private static IResult Order(IOrderService service, IValidator<OrderModel> validator, OrderModel model)
    {
        var validation = validator.Validate(model);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }

        return Results.Ok(service.Order(model));
    }

    private static IResult Success(IOrderService service, ConfigurationOptions options, string sessionId)
    {
        try
        {
            var number = service.OrderSuccess(sessionId);
            return Results.Redirect($"{options.WebsiteUrl}/zamowienie/{number}");
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

    private static IResult GetAll(IOrderService service)
    {
        return Results.Ok(service.GetAll());
    }

    private static IResult Get(IOrderService service, int orderId)
    {
        return Results.Ok(service.Get(orderId));
    }

    private static IResult Update(IOrderService service, AdminOrderModel model)
    {
        return Results.Ok(service.Update(model));
    }

    private static IResult UpdateProduct(IOrderService service, int orderId, AdminProductModel model)
    {
        return Results.Ok(service.UpdateProduct(orderId, model));
    }

    private static IResult GetSingle(IOrderService service, int orderId)
    {
        return Results.Ok(service.GetSingle(orderId));
    }
    
    private static IResult GetStatus(IOrderService service, int orderId)
    {
        return Results.Ok(service.GetStatus(orderId));
    }
}