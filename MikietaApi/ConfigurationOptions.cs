namespace MikietaApi;

public class ConfigurationOptions
{
    public string Database { get; }
    public string SecretKey { get; }
    public string WebsiteUrl { get; }
    public SmtpModel SmtpClient { get; }
    public string GoogleApiKey { get; }
    public string AdminWebsiteUrl { get; }

    public ConfigurationOptions(IConfiguration configuration, IWebHostEnvironment environment,
        ILogger<ConfigurationOptions> logger)
    {
        Database = environment.IsDevelopment()
            ? configuration["ConnectionStrings:Database"]!
            : Environment.GetEnvironmentVariable("Database")!;
        SecretKey = environment.IsDevelopment()
            ? configuration["Stripe:SecretKey"]!
            : Environment.GetEnvironmentVariable("SecretKey")!;
        WebsiteUrl = environment.IsDevelopment()
            ? configuration["WebsiteUrl"]!
            : Environment.GetEnvironmentVariable("WebsiteUrl")!;
        SmtpClient = SmtpModel.Create(configuration, environment);
        GoogleApiKey = environment.IsDevelopment()
            ? configuration["GoogleApiKey"]!
            : Environment.GetEnvironmentVariable("GoogleApiKey")!;
        AdminWebsiteUrl = environment.IsDevelopment()
            ? configuration["AdminWebsiteUrl"]!
            : Environment.GetEnvironmentVariable("AdminWebsiteUrl")!;
    }
}

public class SmtpModel
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public static SmtpModel Create(IConfiguration configuration, IWebHostEnvironment environment)
    {
        return new SmtpModel
        {
            Email = environment.IsDevelopment()
                ? configuration["SmtpClient:Email"]!
                : Environment.GetEnvironmentVariable("SmtpClient_Email")!,
            Host = environment.IsDevelopment()
                ? configuration["SmtpClient:Host"]!
                : Environment.GetEnvironmentVariable("SmtpClient_Host")!,
            Password = environment.IsDevelopment()
                ? configuration["SmtpClient:Password"]!
                : Environment.GetEnvironmentVariable("SmtpClient_Password")!,
            Port = environment.IsDevelopment()
                ? int.Parse(configuration["SmtpClient:Port"]!)
                : int.Parse(Environment.GetEnvironmentVariable("SmtpClient_Port")!)
        };
    }
}