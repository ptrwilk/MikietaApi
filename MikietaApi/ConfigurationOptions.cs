namespace MikietaApi;

public class ConfigurationOptions
{
    public string Database { get; }
    public string SecretKey { get; }
    public string WebsiteUrl { get; }

    public ConfigurationOptions(IConfiguration configuration, IWebHostEnvironment environment, ILogger<ConfigurationOptions> logger)
    {
        Database = environment.IsDevelopment() ? configuration["ConnectionStrings:Database"]! : Environment.GetEnvironmentVariable("Database")!;
        SecretKey = environment.IsDevelopment() ? configuration["Stripe:SecretKey"]! : Environment.GetEnvironmentVariable("SecretKey")!;
        WebsiteUrl = environment.IsDevelopment() ? configuration["WebsiteUrl"]! : Environment.GetEnvironmentVariable("WebsiteUrl")!;
    }
}