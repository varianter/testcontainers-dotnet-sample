using Api.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace Api.TestContainers;

public class TestContainersService(IServiceProvider serviceProvider) : IHostedService
{
    private TestContainers? _testContainers;
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestContainersService>>();
        
        try
        {
            logger.LogInformation("Running test containers service");
            
            var config = scope.ServiceProvider.GetRequiredService<IOptions<TestContainersConfig>>().Value;

            if (config.Enabled)
            {
                var testContainersLogger = scope.ServiceProvider.GetRequiredService<ILogger<TestContainers>>();
                _testContainers = new TestContainers(config, testContainersLogger);

                await _testContainers.Start(cancellationToken);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to start test containers service");
            throw;
        }

        logger.LogInformation("Finished running test containers service");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _testContainers?.Stop(cancellationToken) ?? Task.CompletedTask;
    }
}
