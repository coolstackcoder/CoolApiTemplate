using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

namespace OAuthCore.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OAuthDbContext>
{
    public OAuthDbContext CreateDbContext(string[] args)
    {
        Env.TraversePath().Load();
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        var optionsBuilder = new DbContextOptionsBuilder<OAuthDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new OAuthDbContext(optionsBuilder.Options);
    }
}