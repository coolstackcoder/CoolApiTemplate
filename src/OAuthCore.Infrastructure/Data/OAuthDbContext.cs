using Microsoft.EntityFrameworkCore;
using OAuthCore.Domain.Entities;

namespace OAuthCore.Infrastructure.Data;

public class OAuthDbContext : DbContext
{
    public OAuthDbContext(DbContextOptions<OAuthDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add any additional configuration here
    }
}