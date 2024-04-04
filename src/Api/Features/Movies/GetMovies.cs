using Api.Contracts;
using Api.Database;
using Api.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Movies;

public static class GetMovies
{
    public static async Task<Ok<GetMoviesResponse>> Endpoint(DatabaseContext context, CancellationToken ct)
    {
        var movies = await context.Movies.ToListAsync(ct);

        var response = new GetMoviesResponse
        {
            Movies = movies.Select(x => new GetMoviesResponse.Movie
            {
                Id = x.Id,
                Title = x.Title,
                Year = x.Year
            }).ToList()
        };
        
        return TypedResults.Ok(response);
    }
}