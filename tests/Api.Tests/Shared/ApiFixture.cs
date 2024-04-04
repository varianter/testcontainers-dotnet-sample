namespace Api.Tests.Shared;

[CollectionDefinition(CollectionName)]
public class ApiFixture : ICollectionFixture<ApiFactory>
{
    public const string CollectionName = nameof(ApiFixture);
}