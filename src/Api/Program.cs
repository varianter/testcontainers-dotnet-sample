using Api.Database;
using Api.Features.Movies;
using Api.TestContainers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseContext>();

builder
    .AddDatabaseConfig()
    .AddTestContainersConfig(out var currentTestContainersConfig);

if (currentTestContainersConfig.Enabled)
{
    builder.AddTestContainers();
}

var app = builder.Build();

app.UseSwagger()
    .UseSwaggerUI(opts =>
    {
        if (app.Environment.IsDevelopment())
        {
            opts.EnableTryItOutByDefault();
        }
    });

var movies = app.MapGroup("/movies")
    .WithOpenApi();

movies.MapPost("/", CreateMovie.Endpoint);
movies.MapGet("/", GetMovies.Endpoint);
movies.MapGet("/{id}", GetMovie.Endpoint);

app.Run();

// Make available for testing:
public partial class Program { }