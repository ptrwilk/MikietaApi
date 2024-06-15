namespace MikietaApi.SendEmail.Reservation.Models;

public class ReservationEmailSenderModel : EmailSenderModelBase
{
    public DateTime ReservationDate { get; set; }
    public int NumberOfPeople { get; set; }
}