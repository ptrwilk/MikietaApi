using MikietaApi.SendEmail.Reservation;
using MikietaApi.SendEmail.Reservation.Models;

namespace MikietaApi.Tests.SendEmail;

public class ReservationEmailReplyTest
{
    private class ReservationEmailReplyMock : ReservationEmailReply
    {
        public ReservationEmailReplyMock() : base(null)
        {
            
        }
        
        public new string ReadFromTemplate(ReservationEmailReplyModel model)
        {
            return base.ReadFromTemplate(model);
        }
    }
    
    [Explicit]
    [Test]
    public void ReadFromTemplate()
    {
        var sender = new ReservationEmailReplyMock();

        var content = sender.ReadFromTemplate(new ReservationEmailReplyModel
        {
           Message = "Test"
        });
        
        File.WriteAllText("ReservationEmailReply.html", content);
    }
}