using FluentValidation;
using MikietaApi.Models;
using MikietaApi.Services;

namespace MikietaApi.Routes;

public static class ReservationRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapPost("reservation", Reservation);
        app.MapGet("reservation", GetAll);

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
        
        service.Reserve(model);
        return Results.Ok();
    }
    
    private static IResult GetAll(IReservationService service)
    {
        return Results.Ok(service.GetAll());
    }
}