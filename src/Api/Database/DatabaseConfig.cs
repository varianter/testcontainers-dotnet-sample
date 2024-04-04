namespace Api.Database;

public record DatabaseConfig
{
    public const string SectionName = "Database";
    public string ConnectionString { get; set; }
}

internal static class DatabaseConfigExtensions
{
    internal static IHostApplicationBuilder AddDatabaseConfig(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection(DatabaseConfig.SectionName));
        return builder;
    }
}