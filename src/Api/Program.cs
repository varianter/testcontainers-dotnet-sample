using Api;
using Api.Database;
using Api.TestContainers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

async Task<Created<Movie>> CreateMovie(Movie movie, DatabaseContext context)
{
    context.Movies.Add(movie);
    await context.SaveChangesAsync();
    return TypedResults.Created($"/movies/{movie.Id}", movie);
}

async Task<Ok<List<Movie>>> GetMovies(DatabaseContext context)
{
    var movies = await context.Movies.ToListAsync();
    return TypedResults.Ok(movies);
}

async Task<Ok<Movie>> GetMovie(Guid id, DatabaseContext context)
{
    var movie = await context.Movies.FindAsync(id);
    return TypedResults.Ok(movie);
}

var movies = app.MapGroup("/movies")
    .WithOpenApi();

movies.MapPost("/", CreateMovie);
movies.MapGet("/", GetMovies);
movies.MapGet("/{id}", GetMovie);

app.Run();

public record Movie(Guid Id, string Title, int Year);
