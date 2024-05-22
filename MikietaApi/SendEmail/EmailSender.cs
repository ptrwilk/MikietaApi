using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

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
    private const string Address = "ul.Jakas 2a/3";
    private const string Phone = "+32 100 000 000";
    private const string Web = "www.mikieta.pl";

    public EmailSender(EmailSenderOption option)
    {
        _option = option;
    }
    
    public string Send(EmailSenderModel model)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Pizzeria Siemano", "zenobiusztasak@gmail.com"));
        email.To.Add(new MailboxAddress("Klient", model.RecipientEmail));
        email.Subject = "Rezerwacja w Pizzerii Mikieta";
        
        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = $@"
        <p>Dzień dobry,</p>
        <p>Dziękujemy za dokonanie rezerwacji w naszej Pizzerii Mikieta. Poniżej znajdą Państwo szczegóły rezerwacji:</p>
        <p>Data rezerwacji: {model.ReservationDate:yyyy-MM-dd}</p>
        <p>Godzina rezerwacji: {model.ReservationDate:HH:mm}</p>
        <p>Liczba osób: {model.NumberOfPeople}</p>
        <p>Otrzymają państwo potwierdzenie rezerwacji w kolejnym mailu lub/oraz drogą telefoniczną.</p>
        <p>Z wyrazami szacunku,<br>Zespół Pizzerii Mikieta</p>
        <p>Pizzeria Mikieta<br>
        Adres: {Address}<br>
        Tel.: {Phone}<br>
        {Web}</p>
        ";
        
        email.Body = bodyBuilder.ToMessageBody();
        
        using var smtp = new SmtpClient();
        smtp.Connect(_option.Host, _option.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_option.Email, _option.Password);
        smtp.Send(email);
        smtp.Disconnect(true);

        return email.MessageId;
    }
}