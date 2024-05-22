using System.ComponentModel.DataAnnotations;
using MikietaApi.Models;

namespace MikietaApi.Data.Entities;

public class ReservationEntity
{
    [Key]
    public Guid Id { get; set; }
    public int Number { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ReservationDate { get; set; }
    public int NumberOfPeople { get; set; }
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Comments { get; set; }
    public string? MessageId { get; set; }
    public ReservationStatusType Status { get; set; }
}