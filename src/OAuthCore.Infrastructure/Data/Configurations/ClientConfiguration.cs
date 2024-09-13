using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OAuthCore.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace OAuthCore.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ClientId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.ClientSecret)
            .IsRequired();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.RedirectUris)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        builder.Property(c => c.AllowedGrantTypes)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        builder.Property(c => c.AllowedScopes)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        var stringArrayComparer = new ValueComparer<string[]>(
            (c1, c2) => (c1 != null && c2 != null) && c1.SequenceEqual(c2),
            c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
            c => c != null ? c.ToArray() : new string[0]
        );

        builder.Property(c => c.RedirectUris)
            .Metadata.SetValueComparer(stringArrayComparer);

        builder.Property(c => c.AllowedGrantTypes)
            .Metadata.SetValueComparer(stringArrayComparer);

        builder.Property(c => c.AllowedScopes)
            .Metadata.SetValueComparer(stringArrayComparer);

        builder.HasIndex(c => c.ClientId)
            .IsUnique();
    }
}