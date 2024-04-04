using Api.Database;
using Api.Features.Movies;
using Api.TestContainers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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