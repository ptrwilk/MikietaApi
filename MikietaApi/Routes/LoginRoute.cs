using Microsoft.AspNetCore.Authorization;
using MikietaApi.Models;
using MikietaApi.Services;

namespace MikietaApi.Routes;

public static class LoginRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapPost("login", Login);
        app.MapGet("login/check", Authenticated);

        return app;
    }
    
    private static IResult Login(ILoginService service, LoginModel model)
    {
        try
        {
            return Results.Ok(service.Login(model));
        }
        catch (UnauthorizedAccessException e)
        {
            return Results.Unauthorized();
        }
    }
    
    [Authorize]
    private static IResult Authenticated(ILoginService service)
    {
        return Results.Ok(true);
    }
}