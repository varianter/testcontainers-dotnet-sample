using Api.Database;
using Api.Tests.Shared;

namespace Api.Tests;

public class TestsBase(ApiFactory factory) : IAsyncLifetime
{
    protected readonly HttpClient Client = factory.HttpClient;
    protected readonly DatabaseContext Context = factory.DatabaseContext;
    private readonly Func<Task> ResetDatabaseAsync = factory.ResetDatabaseAsync;
    
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Context.ChangeTracker.Clear();
        return ResetDatabaseAsync();
    }
}