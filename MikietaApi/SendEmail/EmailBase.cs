using System.Net;
using System.Net.Mail;

namespace MikietaApi.SendEmail;

public abstract class EmailBase<T>
    where T : EmailSenderModelBase
{
    private const string RecipientFragmentPath = "SendEmail/Templates/recipient_fragment.html";

    private readonly EmailSenderOption _option;

    protected EmailBase(EmailSenderOption option)
    {
        _option = option;
    }
    
    protected abstract string Subject { get; }
    
    protected abstract string ReadFromTemplate(T model);
    
    protected MailMessage CreateMailMessage(T model, string subject, out string messageId)
    {
        messageId = Guid.NewGuid().ToString();

        return CreateMailMessage(model, subject, messageId);
    }
    
    protected MailMessage CreateMailMessage(T model, string subject, string messageId)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_option.Email, "Pizzeria Mikieta");
        message.To.Add(new MailAddress(model.RecipientEmail, "Klient"));    
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

    protected string ReadFromRecipientFragment(T model)
    {
        var content = File.ReadAllText(RecipientFragmentPath);
        
        content = content.Replace("[ADDRESS]", model.Address);
        content = content.Replace("[PHONE]", model.Phone);
        content = content.Replace("[LINK]", model.Link);
        content = content.Replace("[LINK_TEXT]", model.LinkText);
        
        return content;
    }
}