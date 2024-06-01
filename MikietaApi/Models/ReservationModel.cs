namespace MikietaApi.Models;

public class ReservationModel
{
    public DateTime ReservationDate { get; set; }
    public int NumberOfPeople { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public string? Comments { get; set; }
}