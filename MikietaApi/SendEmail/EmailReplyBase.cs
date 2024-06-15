namespace MikietaApi.SendEmail;

public interface IEmailReply<T>
{
    void Reply(T model);
    
}
public abstract class EmailReplyBase<T> : EmailBase<T>, IEmailReply<T>
    where T : EmailReplayModelBase
{
    protected EmailReplyBase(EmailSenderOption option) : base(option)
    {
    }

    public void Reply(T model)
    {
        var replyMessage = CreateMailMessage(model, $"Re: {Subject}", model.MessageId);
        var htmlBody = ReadFromTemplate(model);
        
        replyMessage.AlternateViews.Add(CreateAlternateView(htmlBody));

        Send(replyMessage);
    }
}