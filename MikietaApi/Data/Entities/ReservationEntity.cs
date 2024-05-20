using System.ComponentModel.DataAnnotations;

namespace MikietaApi.Data.Entities;

public class ReservationEntity
{
    [Key]
    public Guid Id { get; set; }
    public DateTime ReservationDate { get; set; }
    public int NumberOfPeople { get; set; }
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Comments { get; set; }
}