namespace MikietaApi.SendEmail;

public interface IEmailSender<in T>
{
    string Send(T model);
}

public abstract class EmailSenderBase<T> : EmailBase<T>, IEmailSender<T>
{
    protected abstract string Subject { get; }

    protected EmailSenderBase(EmailSenderOption option) : base(option)
    {
    }
    
    public string Send(T model)
    {
        var message = CreateMailMessage(Subject, out var messageId);
        
        var content = ReadFromTemplate(model);
        
        message.AlternateViews.Add(CreateAlternateView(content));

        Send(message);
        
        return messageId;
    }
}