using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace Api.TestContainers;

public class TestContainersService(IServiceProvider serviceProvider) : IHostedService
{
    private const string DatabaseName = "test";
    private const string Username = "test";
    private const string Password = "test123###!";
    private const int HostPort = 51234;

    public static readonly string ConnectionString =
        $"Host=127.0.0.1;Port={HostPort};Database={DatabaseName};Username={Username};Password={Password}";
    
    private PostgreSqlContainer? _postgreSqlContainer;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestContainersService>>();
        try
        {
            logger.LogInformation("Running ephemeral environment service");
            var config = scope.ServiceProvider.GetRequiredService<IOptions<TestContainersConfig>>().Value;

            if (config.Enabled)
            {
                logger.LogInformation("Starting TestContainers");
                _postgreSqlContainer = new PostgreSqlBuilder()
                    .WithName("testcontainers-sample-db")
                    .WithLabel("reuse-id", "testcontainers-sample-db")
                    .WithReuse(true)
                    .WithDatabase(DatabaseName)
                    .WithUsername(Username)
                    .WithPassword(Password)
                    .WithPortBinding(HostPort, 5432)
                    .Build();

                await _postgreSqlContainer.StartAsync(cancellationToken);
            }
        } catch (Exception ex)
        {
            logger.LogError(ex, "Error starting TestContainers");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        var stopTask = _postgreSqlContainer?.StopAsync(cancellationToken) ?? Task.CompletedTask;
        return stopTask;
    }
}
