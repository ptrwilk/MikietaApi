
using MikietaApi.Helpers;

namespace MikietaApi.Services;

public interface IImageService
{
    Guid Add(byte[] bytes);
    byte[] Get(Guid imageId);
}

public class ImageService : IImageService
{
    public Guid Add(byte[] bytes)
    {
        var guid = Guid.NewGuid();
        
        var path = Path.Combine(ResourceHelper.ImagesPath, $"{guid}.png");
        
        File.WriteAllBytes(path, bytes);

        return guid;
    }

    public byte[] Get(Guid imageId)
    {
        return ResourceHelper.GetImage(imageId.ToString());
    }
}