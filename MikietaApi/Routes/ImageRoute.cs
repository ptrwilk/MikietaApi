using MikietaApi.Services;

namespace MikietaApi.Routes;

public static class ImageRoute
{
    public static WebApplication RegisterEndpoints(WebApplication app)
    {
        app.MapGet("image/{imageId}", Get);
        app.MapPost("image", Add);

        return app;
    }
    
    private static IResult Get(IImageService service, Guid imageId)
    {
        return Results.File(service.Get(imageId), "image/png");
    }

    private static async Task<IResult> Add(IImageService service, IFormFile file)
    {
        if (file is { Length: > 0 })
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            return Results.Ok(service.Add(fileBytes));
        }

        return Results.NotFound("Is not an image");
    }
}