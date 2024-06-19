namespace MikietaApi.SendEmail;

public abstract class EmailSenderModelBase
{
    public string RecipientEmail { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Link { get; set; }
    public string? LinkText { get; set; }
}

public abstract class EmailReplayModelBase : EmailSenderModelBase
{
    public string MessageId { get; set; } = null!;
}