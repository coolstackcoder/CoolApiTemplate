using Microsoft.EntityFrameworkCore;
using OAuthCore.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace OAuthCore.Infrastructure.Data;

public class OAuthDbContext : DbContext
{
    public OAuthDbContext(DbContextOptions<OAuthDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<AuthorizationCode> AuthorizationCodes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>()
       .Property(c => c.RedirectUris)
       .HasConversion(
           v => string.Join(',', v),
           v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        modelBuilder.Entity<Client>()
            .Property(c => c.AllowedGrantTypes)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        modelBuilder.Entity<Client>()
            .Property(c => c.AllowedScopes)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        var stringArrayComparer = new ValueComparer<string[]>(
            (c1, c2) => (c1 != null && c2 != null) && c1.SequenceEqual(c2),
            c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
            c => c != null ? c.ToArray() : new string[0]
        );

        modelBuilder.Entity<Client>()
            .Property(c => c.RedirectUris)
            .Metadata.SetValueComparer(stringArrayComparer);

        modelBuilder.Entity<Client>()
            .Property(c => c.AllowedGrantTypes)
            .Metadata.SetValueComparer(stringArrayComparer);

        modelBuilder.Entity<Client>()
            .Property(c => c.AllowedScopes)
            .Metadata.SetValueComparer(stringArrayComparer);
    }
}