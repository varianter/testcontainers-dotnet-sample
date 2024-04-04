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

namespace Api.Tests.Shared;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public TestContainers.TestContainers TestContainers { get; private set; } = default!;
    public HttpClient HttpClient { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        var config = new TestContainersConfig
        {
            Enabled = true,
            RunMigrations = true
        };
        
        TestContainers = new TestContainers.TestContainers(config, NullLogger<TestContainers.TestContainers>.Instance);
        await TestContainers.Start(overrides: new TestContainersOverrides
        {
            DatabaseTestContainerName = "testcontainers-sample-api-tests-db",
            HostPort = 51235
        });
        HttpClient = CreateClient();
    }

    public new Task DisposeAsync()
    {
        HttpClient.Dispose();
        return TestContainers.Stop();
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
                options.ConnectionString = TestContainers.CurrentConnectionString ?? throw new UnreachableException();
            });
        });
    }
}