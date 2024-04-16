using FluentValidation;
using MikietaApi.Models;
using MikietaApi.Services;

namespace MikietaApi.Routes;

public static class OrderRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapPost("order", Order);
        
        return app;
    }
    
    private static IResult Order(IOrderService service, IValidator<OrderModel> validator, OrderModel model)
    {
        var validation = validator.Validate(model);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }
        
        service.Order(model);
        return Results.Ok();
    }
}