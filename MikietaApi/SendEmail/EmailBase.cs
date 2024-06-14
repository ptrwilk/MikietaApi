using System.Net;
using System.Net.Mail;

namespace MikietaApi.SendEmail;

public abstract class EmailBase<T>
{
    private readonly EmailSenderOption _option;

    protected EmailBase(EmailSenderOption option)
    {
        _option = option;
    }
    
    protected abstract string ReadFromTemplate(T model);
    
    protected MailMessage CreateMailMessage(string subject, out string messageId)
    {
        messageId = Guid.NewGuid().ToString();

        return CreateMailMessage(subject, messageId);
    }
    
    protected MailMessage CreateMailMessage(string subject, string messageId)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_option.Email, "Pizzeria Mikieta");
        message.To.Add(new MailAddress("ptrwilk@outlook.com", "Klient")); //TODO: Replace email with one from model later       
        message.Subject = subject;

        message.Headers.Add("In-Reply-To", messageId);
        message.Headers.Add("References", messageId);

        return message;
    }
    
    protected AlternateView CreateAlternateView(string body)
    {
        return AlternateView.CreateAlternateViewFromString(body, null, "text/html");
    }
    
    protected void Send(MailMessage message)
    {
        using SmtpClient smtpClient = new SmtpClient(_option.Host, _option.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_option.Email, _option.Password)
        };
        
        smtpClient.Send(message);
    }
}