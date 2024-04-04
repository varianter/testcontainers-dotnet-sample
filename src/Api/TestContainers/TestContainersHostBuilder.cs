using Api.Database;

namespace Api.TestContainers;

public static class TestContainersHostBuilder
{
    public static void AddTestContainers(this IHostApplicationBuilder builder)
    {
        if (!builder.Environment.IsValidTestContainersEnvironment())
            throw new InvalidOperationException(
                $"{nameof(AddTestContainers)} should only be called in non-live environments like Development. Current environment: {builder.Environment.EnvironmentName}");

        builder.Services.AddHostedService<TestContainersService>();
        
        builder.Services.Configure<DatabaseConfig>(
            opts =>
            {
                opts.ConnectionString = TestContainersService.ConnectionString;
            });
    }

    private static bool IsValidTestContainersEnvironment(this IHostEnvironment environment)
    {
        return environment.IsDevelopment();
    }
}