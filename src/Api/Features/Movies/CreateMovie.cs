using Api.Contracts;
using Api.Database;
using Api.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Movies;

public static class CreateMovie
{
    public static async Task<Created> Endpoint([FromBody] CreateMovieRequest request, DatabaseContext context, CancellationToken ct)
    {
        var movie = new Movie(Guid.NewGuid(), request.Title, request.Year);

        context.Movies.Add(movie);
        await context.SaveChangesAsync(ct);
        
        return TypedResults.Created($"/movies/{movie.Id}");
    }
}