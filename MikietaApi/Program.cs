using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MikietaApi;
using MikietaApi.Data;
using MikietaApi.Models;
using MikietaApi.Routes;
using MikietaApi.Services;
using MikietaApi.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration["ConnectionStrings:Database"]));
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<DbSeeder, DbSeeder>();
builder.Services.AddScoped<IValidator<OrderModel>, OrderModelValidator>();

builder.Services.AddCors(options =>
    options.AddPolicy("MyPolicy",
        b => { b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }
    )
);

var app = builder.Build();

app.MapSwagger();
app.UseSwaggerUI();

app.UseCors("MyPolicy");

ProductsRoute.RegisterEndpoints(app);
OrderRoute.RegisterEndpoints(app);

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetService<DbSeeder>();
seeder!.Seed();

app.Run();