﻿using FluentValidation;
using GoogleMaps.LocationServices;
using MikietaApi.Models;
using MikietaApi.Services;

namespace MikietaApi.Routes;

public static class DeliveryRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapPost("delivery/check", Check);

        return app;
    }
    
    private static IResult Check(IDeliveryService service, IValidator<DeliveryModel> validator, DeliveryModel model)
    {
        try
        {
            var validation = validator.Validate(model);
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }
            
            return Results.Ok(service.CheckDistance(model));
        }
        catch (DeliveryCheckException e)
        {
            return Results.NotFound(e.Error);
        }
    }
}