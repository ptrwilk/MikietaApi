using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace MikietaApi.Tests;

public class WebAppFactory : WebApplicationFactory<Program>
{
    private Action<IServiceCollection>? ConfigureServices { get; set; }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        builder.ConfigureHostConfiguration(c =>
        {
            c.AddInMemoryCollection(new Dictionary<string, string?>
            {
                //Everything that relies on builder[Configuration] must be placed here
            });
        });

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = config["ConnectionStrings:Database"],
                ["Stripe:SecretKey"] = config["Stripe:SecretKey"],
                ["WebsiteUrl"] = config["WebsiteUrl"],
                ["SmtpClient:Email"] = config["SmtpClient:Email"],
                ["SmtpClient:Host"] = config["SmtpClient:Host"],
                ["SmtpClient:Password"] = config["SmtpClient:Password"],
                ["SmtpClient:Port"] = config["SmtpClient:Port"],
            }!);
        });
        
        builder.ConfigureServices(services =>
        {
            ConfigureServices?.Invoke(services);
        });

        return base.CreateHost(builder);
    }
    
    public static WebAppFactory CreateFactory(Action<IServiceCollection> serviceCollection)
    {
        var factory = new WebAppFactory();
        
        factory.ConfigureServices = serviceCollection;

        return factory;
    }
}