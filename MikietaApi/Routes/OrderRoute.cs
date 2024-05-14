using FluentValidation;
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
        app.MapGet("order/{orderId}", Get);

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
}