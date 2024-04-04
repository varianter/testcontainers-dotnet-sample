using System.Net;
using System.Net.Http.Json;
using Api.Contracts;
using Api.Tests.Shared;
using Bogus;
using FluentAssertions;

namespace Api.Tests;

[Collection(ApiFixture.CollectionName)]
public class CreateMovieTests(ApiFactory factory)
{
    private HttpClient Client = factory.HttpClient;
    private static Faker<CreateMovieRequest> CreateMovieRequestFaker = new Faker<CreateMovieRequest>()
        .RuleFor(x => x.Title, f => f.Random.String2(1, 100))
        .RuleFor(x => x.Year, f => f.Random.Int(1900, 2100));
    
    [Fact]
    public async Task CreateMovie_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = CreateMovieRequestFaker.Generate();

        // Act
        var response = await Client.PostAsJsonAsync("/movies", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }
}