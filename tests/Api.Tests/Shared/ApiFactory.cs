using System.Data.Common;
using System.Diagnostics;
using Api.Database;
using Api.TestContainers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql;
using Respawn;

namespace Api.Tests.Shared;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public TestContainers.TestContainersFactory TestContainersFactory { get; private set; } = default!;
    public DatabaseContext DatabaseContext { get; private set; } = default!;
    public HttpClient HttpClient { get; private set; } = default!;
    
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;


    public async Task InitializeAsync()
    {
        var config = new TestContainersConfig
        {
            Enabled = true,
            RunMigrations = true
        };
        
        TestContainersFactory = new TestContainers.TestContainersFactory(config, NullLogger<TestContainers.TestContainersFactory>.Instance);
        await TestContainersFactory.Start(overrides: new TestContainersOverrides
        {
            DatabaseTestContainerName = "testcontainers-sample-api-tests-db",
            HostPort = 51235
        });
        DatabaseContext = new DatabaseContext(Options.Create(new DatabaseConfig
        {
            ConnectionString = TestContainersFactory.CurrentConnectionString ?? throw new UnreachableException(),
            EnableSensitiveDataLogging = true
        }), NullLoggerFactory.Instance);
        
        HttpClient = CreateClient();
        
        _dbConnection = new NpgsqlConnection(TestContainersFactory.CurrentConnectionString);
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
        });
    }
    
    public Task ResetDatabaseAsync()
    {
        return _respawner.ResetAsync(_dbConnection);
    }

    public new Task DisposeAsync()
    {
        HttpClient.Dispose();
        return TestContainersFactory.Stop();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.json");
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(TestContainersService));
            services.Configure<DatabaseConfig>(options =>
            {
                options.ConnectionString = TestContainersFactory.CurrentConnectionString ?? throw new UnreachableException();
            });
        });
    }
}