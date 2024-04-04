using Api.Contracts;
using Api.Database;
using Api.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Movies;

public static class GetMovie
{
    public static async Task<Results<Ok<GetMovieResponse>, NotFound>> Endpoint(Guid id, DatabaseContext context, CancellationToken ct)
    {
        var movie = await context.Movies.FirstOrDefaultAsync(m => m.Id == id, ct);
        
        if (movie is null)
        {
            return TypedResults.NotFound();
        }
        
        var response = new GetMovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year
        };
        
        return TypedResults.Ok(response);
    }
}