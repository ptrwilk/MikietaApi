using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MikietaApi;
using MikietaApi.Converters;
using MikietaApi.Data;
using MikietaApi.Data.Entities;
using MikietaApi.Models;
using MikietaApi.Routes;
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
builder.Services.AddScoped<DbSeeder, DbSeeder>();
builder.Services.AddScoped<IValidator<OrderModel>, OrderModelValidator>();
builder.Services.AddScoped<IValidator<ReservationModel>, ReservationModelValidator>();
builder.Services.AddSingleton<ConfigurationOptions, ConfigurationOptions>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IConverter<OrderProductEntity, StripeRequestModel>, StripeRequestConverter>();
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

var app = builder.Build();

app.MapSwagger();
app.UseSwaggerUI();

StripeConfiguration.ApiKey = app.Services.GetService<ConfigurationOptions>()!.SecretKey;

app.UseCors("MyPolicy");

ProductsRoute.RegisterEndpoints(app);
OrderRoute.RegisterEndpoints(app);
ReservationRoute.RegisterEndpoints(app);

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetService<DbSeeder>();
seeder!.Seed();

app.Run();