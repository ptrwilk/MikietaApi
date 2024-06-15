namespace MikietaApi.SendEmail;

public interface IEmailSender<in T>
{
    string Send(T model);
}

public abstract class EmailSenderBase<T> : EmailBase<T>, IEmailSender<T>
    where T : EmailSenderModelBase
{

    protected EmailSenderBase(EmailSenderOption option) : base(option)
    {
    }
    
    public string Send(T model)
    {
        var message = CreateMailMessage(model, Subject, out var messageId);
        
        var content = ReadFromTemplate(model);
        
        message.AlternateViews.Add(CreateAlternateView(content));

        Send(message);
        
        return messageId;
    }
}