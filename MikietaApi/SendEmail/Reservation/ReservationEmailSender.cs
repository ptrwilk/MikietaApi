using MikietaApi.SendEmail.Reservation.Models;

namespace MikietaApi.SendEmail.Reservation;

public class ReservationEmailSender : EmailSenderBase<ReservationEmailSenderModel>
{
    private const string Path = "SendEmail/Reservation/Templates/send_email_template.html";
    
    protected override string Subject => "Rezerwacja w Pizzerii Mikieta";

    public ReservationEmailSender(EmailSenderOption option) : base(option)
    {
    }

    protected override string ReadFromTemplate(ReservationEmailSenderModel model)
    {
        var content = File.ReadAllText(Path);
        
        content = content.Replace("[DATE]", model.ReservationDate.ToString("yyyy-MM-dd"));
        content = content.Replace("[TIME]", model.ReservationDate.ToString("HH:mm"));
        content = content.Replace("[GUESTS]", model.NumberOfPeople.ToString());

        return content;
    }
}