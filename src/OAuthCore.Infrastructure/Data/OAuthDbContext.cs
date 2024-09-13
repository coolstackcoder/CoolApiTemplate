using Microsoft.EntityFrameworkCore;
using OAuthCore.Application.Data;
using OAuthCore.Domain.Entities;

namespace OAuthCore.Infrastructure.Data;

public class OAuthDbContext : DbContext, IDbContext
{
    public OAuthDbContext(DbContextOptions<OAuthDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<AuthorizationCode> AuthorizationCodes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OAuthDbContext).Assembly);
    }
}