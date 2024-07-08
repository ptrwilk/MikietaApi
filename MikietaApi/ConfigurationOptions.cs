namespace MikietaApi;

public class ConfigurationOptions
{
    public string Database { get; }
    public string SecretKey { get; }
    public string WebsiteUrl { get; }
    public SmtpModel SmtpClient { get; }
    public string GoogleApiKey { get; }
    public string AdminWebsiteUrl { get; }

    public ConfigurationOptions(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Database = environment.IsDevelopment()
            ? ConvertPostgresConnectionString(configuration["ConnectionStrings:Database"]!)
            //DATABASE_URL - key  for heroku environment variable
            : ConvertPostgresConnectionString(Environment.GetEnvironmentVariable("DATABASE_URL")!);
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

    public static string ConvertPostgresConnectionString(string postgresUrl)
    {
        try
        {
            var uri = new Uri(postgresUrl);
            var userInfo = uri.UserInfo.Split(':');

            var host = uri.Host;
            var port = uri.Port;
            var database = uri.AbsolutePath.TrimStart('/');
            var username = userInfo[0];
            var password = userInfo[1];

            return
                $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing connection string: {ex.Message}");
            return null;
        }
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