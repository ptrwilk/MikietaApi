using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using MikietaApi.Models;
using MikietaApi.Services;

namespace MikietaApi.Routes;

public static class ReservationRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapPost("reservation", Reservation);
        app.MapGet("reservation", GetAll);
        app.MapPut("reservation", Update);
        app.MapPut("reservation/sendEmail", SendEmail);

        return app;
    }

    private static IResult Reservation(IReservationService service, IValidator<ReservationModel> validator,
        ReservationModel model)
    {
        var validation = validator.Validate(model);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }
        
        return Results.Ok(service.Reserve(model));
    }
    
    [Authorize]
    private static IResult GetAll(IReservationService service)
    {
        return Results.Ok(service.GetAll());
    }
    
    [Authorize]
    private static IResult Update(IReservationService service, ReservationModel model)
    {
        return Results.Ok(service.Update(model));
    }
    
    [Authorize]
    private static IResult SendEmail(IReservationService service, SendEmailModel model)
    {
        return Results.Ok(service.SendEmail(model));
    }
}