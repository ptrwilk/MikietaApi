namespace MikietaApi.SendEmail;

public abstract class EmailSenderModelBase
{
    public string RecipientEmail { get; set; } = null!;
}

public abstract class EmailReplayModelBase : EmailSenderModelBase
{
    public string MessageId { get; set; } = null!;
}