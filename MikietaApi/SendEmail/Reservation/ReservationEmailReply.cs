using MikietaApi.SendEmail.Reservation.Models;

namespace MikietaApi.SendEmail.Reservation;

public class ReservationEmailReply : EmailReplyBase<ReservationEmailReplyModel>
{
    private const string Path = "SendEmail/Reservation/Templates/reply_email_template.html";
    
    protected override string Subject => "Rezerwacja w Pizzerii Mikieta";

    public ReservationEmailReply(EmailSenderOption option) : base(option)
    {
        
    }

    protected override string ReadFromTemplate(ReservationEmailReplyModel model)
    {
        var content = File.ReadAllText(Path);
        
        content = content.Replace("[TEXT]", model.Message);

        return content;
    }
}