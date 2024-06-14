namespace MikietaApi.SendEmail;

public interface IEmailReply<T>
{
    void Reply(string messageId, T message, string recipientEmail);
    
}
public abstract class EmailReplyBase<T> : EmailBase<T>, IEmailReply<T>
{
    protected EmailReplyBase(EmailSenderOption option) : base(option)
    {
    }

    public void Reply(string messageId, T message, string recipientEmail)
    {
        var replyMessage = CreateMailMessage("Re: Rezerwacja w Pizzerii Mikieta", messageId);
        var htmlBody = ReadFromTemplate(message);
        
        replyMessage.AlternateViews.Add(CreateAlternateView(htmlBody));

        Send(replyMessage);
    }
}