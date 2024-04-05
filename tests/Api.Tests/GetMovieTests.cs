using System.Net;
using System.Net.Http.Json;
using Api.Contracts;
using Api.Database;
using Api.Entities;
using Api.Tests.Shared;
using Bogus;
using FluentAssertions;

namespace Api.Tests;

[Collection(ApiFixture.CollectionName)]
public class GetMovieTests(ApiFactory factory) : TestsBase(factory)
{
    private static Faker<Movie> CreateMovieRequestFaker = new Faker<Movie>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.Title, f => f.Random.String2(1, 100))
        .RuleFor(x => x.Year, f => f.Random.Int(1900, 2100));
    
    [Fact] 
    public async Task GetMovie_WithValidId_ReturnsOk()
    {
        // Arrange
        var movie = CreateMovieRequestFaker.Generate();
        Context.Movies.Add(movie);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/movies/{movie.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseMovie = await response.Content.ReadFromJsonAsync<GetMovieResponse>();
        responseMovie.Should().BeEquivalentTo(movie);
    }
}