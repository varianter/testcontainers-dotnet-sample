using Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api.Database;

public class DatabaseContext(IOptions<DatabaseConfig> options, ILoggerFactory loggerFactory) : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(options.Value.ConnectionString)
            .UseLoggerFactory(loggerFactory)
            .EnableSensitiveDataLogging(options.Value.EnableSensitiveDataLogging);
}