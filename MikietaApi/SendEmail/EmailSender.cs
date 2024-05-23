using System.Net;
using System.Net.Mail;

namespace MikietaApi.SendEmail;

public class EmailSenderOption
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class EmailSenderModel
{
    public string RecipientEmail { get; set; } = null!;
    public DateTime ReservationDate { get; set; }
    public int NumberOfPeople { get; set; }
}

public class EmailSender
{
    private readonly EmailSenderOption _option;

    public EmailSender(EmailSenderOption option)
    {
        _option = option;
    }

    public string Send(EmailSenderModel model)
    {
        var message = CreateMailMessage("Rezerwacja w Pizzerii Mikieta", out var messageId);
        
        var htmlBody = SendEmailTemplateReader.Read(new SendEmailTemplateReaderModel
        {
            Date = model.ReservationDate.ToString("yyyy-MM-dd"),
            Time = model.ReservationDate.ToString("HH:mm"),
            Guests = model.NumberOfPeople.ToString(),
        });
        message.AlternateViews.Add(CreateAlternateView(htmlBody));

        Send(message);

        return messageId;
    }


    public void Reply(string messageId, string message, string recipientEmail)
    {
        var replyMessage = CreateMailMessage("Re: Rezerwacja w Pizzerii Mikieta", messageId);
        var htmlBody = ReplyEmailTemplateReader.Read(message);
        
        replyMessage.AlternateViews.Add(CreateAlternateView(htmlBody));

        Send(replyMessage);
    }

    private AlternateView CreateAlternateView(string body)
    {
        return AlternateView.CreateAlternateViewFromString(body, null, "text/html");
    }

    private MailMessage CreateMailMessage(string subject, out string messageId)
    {
        messageId = Guid.NewGuid().ToString();

        return CreateMailMessage(subject, messageId);
    }
    
    private MailMessage CreateMailMessage(string subject, string messageId)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_option.Email, "Pizzeria Mikieta");
        message.To.Add(new MailAddress("ptrwilk@outlook.com", "Klient")); //TODO: Replace email with one from model later       
        message.Subject = subject;

        message.Headers.Add("In-Reply-To", messageId);
        message.Headers.Add("References", messageId);

        return message;
    }

    private void Send(MailMessage message)
    {
        using SmtpClient smtpClient = new SmtpClient(_option.Host, _option.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_option.Email, _option.Password)
        };
        
        smtpClient.Send(message);
    }
}

public class SendEmailTemplateReaderModel
{
    public string Date { get; set; }
    public string Time { get; set; }
    public string Guests { get; set; }
}

public static class SendEmailTemplateReader
{
    private const string Path = "SendEmail/send_email_template.html";
    
    public static string Read(SendEmailTemplateReaderModel model)
    {
        var content = File.ReadAllText(Path);
        
        content = content.Replace("[DATE]", model.Date);
        content = content.Replace("[TIME]", model.Time);
        content = content.Replace("[GUESTS]", model.Guests);

        return content;
    }
}

public static class ReplyEmailTemplateReader
{
    private const string Path = "SendEmail/reply_email_template.html";
    
    public static string Read(string text)
    {
        var content = File.ReadAllText(Path);
        
        content = content.Replace("[TEXT]", text);

        return content;
    }
}