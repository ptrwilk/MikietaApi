using System.ComponentModel.DataAnnotations;

namespace MikietaApi.Data.Entities;

public class ImageEntity
{
    [Key]
    public Guid Id { get; set; }
    public byte[] Bytes { get; set; } = null!;
}