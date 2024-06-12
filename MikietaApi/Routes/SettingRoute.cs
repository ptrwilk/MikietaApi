using Microsoft.AspNetCore.Authorization;
using MikietaApi.Models;
using MikietaApi.Services;

namespace MikietaApi.Routes;

public static class SettingRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapGet("setting", Get);
        app.MapPut("setting", Update);

        return app;
    }
    
    [Authorize]
    private static IResult Get(ISettingService service)
    {
        return Results.Ok(service.Get());
    }
    
    [Authorize]
    private static IResult Update(ISettingService service, SettingModel model)
    {
        return Results.Ok(service.Update(model));
    }
}