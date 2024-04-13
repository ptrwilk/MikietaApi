using System.Data;
using System.Data.SQLite;
using MikietaApi;
using MikietaApi.Repositories;
using MikietaApi.Routes;
using MikietaApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbConnection>(_ => new SQLiteConnection(builder.Configuration["ConnectionStrings:Database"]));
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<IIngredientsRepository, IngredientsRepository>();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<DbSeeder, DbSeeder>();

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

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetService<DbSeeder>();
seeder!.Seed();

app.Run();