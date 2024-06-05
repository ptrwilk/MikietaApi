using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MikietaApi;
using MikietaApi.Converters;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Hubs;
using MikietaApi.Models;
using MikietaApi.Routes;
using MikietaApi.SendEmail;
using MikietaApi.Services;
using MikietaApi.Stripe;
using MikietaApi.Validators;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>((provider, options) =>
    options.UseSqlite(provider.GetService<ConfigurationOptions>()!.Database));
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<DbSeeder, DbSeeder>();
builder.Services.AddScoped<IValidator<OrderModel>, OrderModelValidator>();
builder.Services.AddScoped<IValidator<ReservationModel>, ReservationModelValidator>();
builder.Services.AddScoped<IValidator<AdditionalIngredientModel>, AdditionalIngredientModelValidator>();
builder.Services.AddScoped<IValidator<RemovedIngredientModel>, RemovedIngredientModelValidator>();
builder.Services.AddScoped<IValidator<ReplacedIngredientModel>, ReplacedIngredientModelValidator>();
builder.Services.AddSingleton<ConfigurationOptions, ConfigurationOptions>();
builder.Services.AddScoped<EmailSender, EmailSender>(provider =>
{
    var smtpClient = provider.GetService<ConfigurationOptions>()!.SmtpClient;
    return new EmailSender(new EmailSenderOption
    {
        Host = smtpClient.Host,
        Email = smtpClient.Email,
        Password = smtpClient.Password,
        Port = smtpClient.Port
    });
});

builder.Services.AddHttpContextAccessor();

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

builder.Services.AddSignalR();

var app = builder.Build();

app.MapSwagger();
app.UseSwaggerUI();

StripeConfiguration.ApiKey = app.Services.GetService<ConfigurationOptions>()!.SecretKey;

app.UseCors("MyPolicy");

app.MapHub<MessageHub>("/messageHub");

ProductsRoute.RegisterEndpoints(app);
OrderRoute.RegisterEndpoints(app);
ReservationRoute.RegisterEndpoints(app);
IngredientRoute.RegisterEndpoints(app);
ImageRoute.RegisterEndpoints(app);

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetService<DbSeeder>();
seeder!.Seed();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program
{
}