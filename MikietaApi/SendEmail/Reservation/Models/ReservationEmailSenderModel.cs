namespace MikietaApi.SendEmail.Reservation.Models;

public class ReservationEmailSenderModel
{
    public string RecipientEmail { get; set; } = null!;
    public DateTime ReservationDate { get; set; }
    public int NumberOfPeople { get; set; }
}