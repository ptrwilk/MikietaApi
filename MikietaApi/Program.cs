using FluentValidation;
using GoogleMaps.LocationServices;
using Jwt.Core;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MikietaApi;
using MikietaApi.Converters;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Factories;
using MikietaApi.Hubs;
using MikietaApi.Models;
using MikietaApi.Routes;
using MikietaApi.SendEmail;
using MikietaApi.SendEmail.Order;
using MikietaApi.SendEmail.Reservation;
using MikietaApi.SendEmail.Reservation.Models;
using MikietaApi.Services;
using MikietaApi.Stripe;
using MikietaApi.Validators;
using Serilog;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerWithJwt();

builder.Services.AddSingleton<IJwtTokenFactory>(_ => new JwtTokenFactory(builder.Configuration["Jwt:Key"]!));
builder.Services.AddDbContext<DataContext>((provider, options) =>
    options.UseSqlite(provider.GetService<ConfigurationOptions>()!.Database));
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<DbSeeder, DbSeeder>();
builder.Services.AddScoped<IValidator<OrderModel>, OrderModelValidator>();
builder.Services.AddScoped<IValidator<ReservationModel>, ReservationModelValidator>();
builder.Services.AddScoped<IValidator<AdditionalIngredientModel>, AdditionalIngredientModelValidator>();
builder.Services.AddScoped<IValidator<RemovedIngredientModel>, RemovedIngredientModelValidator>();
builder.Services.AddScoped<IValidator<ReplacedIngredientModel>, ReplacedIngredientModelValidator>();
builder.Services.AddScoped<IValidator<DeliveryModel>, DeliveryModelValidator>();
builder.Services.AddSingleton<ConfigurationOptions, ConfigurationOptions>();
builder.Services.AddSingleton<IEmailSenderModelFactory, EmailSenderModelFactory>();
builder.Services.AddSingleton<EmailSenderOption, EmailSenderOption>(provider =>
{
    var smtpClient = provider.GetService<ConfigurationOptions>()!.SmtpClient;
    return new EmailSenderOption
    {
        Host = smtpClient.Host,
        Email = smtpClient.Email,
        Password = smtpClient.Password,
        Port = smtpClient.Port
    };
});
builder.Services.AddScoped<IEmailSender<ReservationEmailSenderModel>, ReservationEmailSender>();
builder.Services.AddScoped<IEmailReply<ReservationEmailReplyModel>, ReservationEmailReply>();
builder.Services.AddScoped<IEmailSender<OrderEmailSenderModel>, OrderEmailSender>();
builder.Services.AddScoped<GoogleLocationService, GoogleLocationService>(provider =>
{
    var googleApiKey = provider.GetService<ConfigurationOptions>()!.GoogleApiKey;
    return new GoogleLocationService(googleApiKey);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthenticationWithJwt(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthorization();

builder.Services.AddScoped<IConverter<OrderOrderedProductEntity, StripeRequestModel>, StripeRequestConverter>();
builder.Services.AddScoped<StripeFacade, StripeFacade>(x =>
{
    var context = x.GetRequiredService<IHttpContextAccessor>().HttpContext!;

    var url = $"{context.Request.Scheme}://{context.Request.Host}";
    var successUrl = $"{url}/order/success?sessionId={{CHECKOUT_SESSION_ID}}";
    var cancelUrl = $"{url}/order/cancel";

    return new StripeFacade(successUrl, cancelUrl);
});

builder.Services.AddCors(options =>
    options.AddPolicy("MyPolicy",
        b => { b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }
    )
);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSignalR();

var app = builder.Build();

app.MapSwagger();
app.UseSwaggerUI();

StripeConfiguration.ApiKey = app.Services.GetService<ConfigurationOptions>()!.SecretKey;

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("MyPolicy");
app.UseSerilogRequestLogging();

app.MapHub<MessageHub>("/messageHub");

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

        if (contextFeature is not null)
        {
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error"
            });
        }
    });
});

ProductsRoute.RegisterEndpoints(app);
OrderRoute.RegisterEndpoints(app);
ReservationRoute.RegisterEndpoints(app);
IngredientRoute.RegisterEndpoints(app);
ImageRoute.RegisterEndpoints(app);
DeliveryRoute.RegisterEndpoints(app);
SettingRoute.RegisterEndpoints(app);
LoginRoute.RegisterEndpoints(app);

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetService<DbSeeder>();
seeder!.Seed();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program
{
}