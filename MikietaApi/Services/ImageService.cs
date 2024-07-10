
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Helpers;

namespace MikietaApi.Services;

public interface IImageService
{
    Guid Add(byte[] bytes);
    byte[] Get(Guid imageId);
}

public class ImageService : IImageService
{
    private readonly DataContext _context;

    public ImageService(DataContext context)
    {
        _context = context;
    }
    
    public Guid Add(byte[] bytes)
    {
        var guid = Guid.NewGuid();

        _context.Images.Add(new ImageEntity
        {
            Id = guid,
            Bytes = bytes
        });

        _context.SaveChanges();

        return guid;
    }

    public byte[] Get(Guid imageId)
    {
        return _context.Images.First(x => x.Id == imageId).Bytes;
    }
}