using Microsoft.EntityFrameworkCore;
using OAuthCore.Application.Data;

namespace OAuthCore.Infrastructure.Data;

public interface IDatabaseFactory
{
    IDbContext CreateDbContext();
}

public class PostgresDatabaseFactory : IDatabaseFactory
{
    private readonly DbContextOptions<OAuthDbContext> _options;

    public PostgresDatabaseFactory(DbContextOptions<OAuthDbContext> options)
    {
        _options = options;
    }

    public IDbContext CreateDbContext()
    {
        return new OAuthDbContext(_options);
    }
}