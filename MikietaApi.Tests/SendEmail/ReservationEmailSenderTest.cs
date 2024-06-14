using MikietaApi.SendEmail.Reservation;
using MikietaApi.SendEmail.Reservation.Models;

namespace MikietaApi.Tests.SendEmail;

public class ReservationEmailSenderTest
{
    private class ReservationEmailSenderMock : ReservationEmailSender
    {
        public ReservationEmailSenderMock() : base(null)
        {
            
        }
        
        public new string ReadFromTemplate(ReservationEmailSenderModel model)
        {
            return base.ReadFromTemplate(model);
        }
    }
    
    [Explicit]
    [Test]
    public void ReadFromTemplate()
    {
        var sender = new ReservationEmailSenderMock();

        var content = sender.ReadFromTemplate(new ReservationEmailSenderModel
        {
            NumberOfPeople = 5,
            RecipientEmail = "some@gmail.com",
            ReservationDate = DateTime.Now
        });
        
        File.WriteAllText("ReservationEmailSender.html", content);
    }
}