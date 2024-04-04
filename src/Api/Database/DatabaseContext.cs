using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api.Database;

public class DatabaseContext(IOptions<DatabaseConfig> options) : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(options.Value.ConnectionString);
}