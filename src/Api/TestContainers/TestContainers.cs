using Api.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace Api.TestContainers;

public class TestContainers(TestContainersConfig config, ILogger<TestContainers> logger)
{
    private const string DatabaseName = "test";
    private const string Username = "test";
    private const string Password = "test123###!";
    private const int HostPort = 51234;

    public static readonly string DefaultDbConnectionString =
        $"Host=127.0.0.1;Port={HostPort};Database={DatabaseName};Username={Username};Password={Password}";
    
    private PostgreSqlContainer? _postgreSqlContainer;

    public string? CurrentConnectionString => _postgreSqlContainer?.GetConnectionString();

    public async Task Start(CancellationToken cancellationToken = default, TestContainersOverrides? overrides = null)
    {
        try
        {
            logger.LogInformation("Running ephemeral environment service");

            if (config.Enabled)
            {
                var hostPort = overrides?.HostPort ?? HostPort;
                var databaseTestContainerName = overrides?.DatabaseTestContainerName ?? "testcontainers-sample-db";

                logger.LogInformation("Starting TestContainers");
                _postgreSqlContainer = new PostgreSqlBuilder()
                    .WithName(databaseTestContainerName)
                    .WithLabel("reuse-id", databaseTestContainerName)
                    .WithReuse(true)
                    .WithDatabase(DatabaseName)
                    .WithUsername(Username)
                    .WithPassword(Password)
                    .WithPortBinding(hostPort, 5432)
                    .Build();

                await _postgreSqlContainer.StartAsync(cancellationToken);
                
                if (config.RunMigrations)
                {
                    var options = Options.Create(new DatabaseConfig
                    {
                        ConnectionString = _postgreSqlContainer.GetConnectionString(),
                        EnableSensitiveDataLogging = true
                    });

                    await using var context = new DatabaseContext(options, NullLoggerFactory.Instance);

                    var retries = 0;
                    while (!await context.Database.CanConnectAsync(cancellationToken) && retries < 10)
                    {
                        retries++;
                        await Task.Delay(1000, cancellationToken);
                    }

                    logger.LogInformation("Running database migrations");
                    await context.Database.MigrateAsync(cancellationToken); 
                }
            }
        } catch (Exception ex)
        {
            logger.LogError(ex, "Error starting TestContainers");
        }
    }

    public Task Stop(CancellationToken cancellationToken = default)
    {
        var stopTask = _postgreSqlContainer?.StopAsync(cancellationToken) ?? Task.CompletedTask;
        return stopTask;
    }

}

public class TestContainersOverrides
{
    public string? DatabaseTestContainerName { get; set; }
    public int? HostPort { get; set; }
}