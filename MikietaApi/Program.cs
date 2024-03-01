var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddPolicy("MyPolicy",
        b =>
        {
            b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    )
);

var app = builder.Build();

app.UseCors("MyPolicy");

app.MapGet("/", () => new []{"a", "b", "c"});

app.Run();