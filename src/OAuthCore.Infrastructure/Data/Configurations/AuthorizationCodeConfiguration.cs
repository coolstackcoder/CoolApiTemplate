using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OAuthCore.Domain.Entities;

namespace OAuthCore.Infrastructure.Data.Configurations;

public class AuthorizationCodeConfiguration : IEntityTypeConfiguration<AuthorizationCode>
{
    public void Configure(EntityTypeBuilder<AuthorizationCode> builder)
    {
        builder.HasKey(ac => ac.Id);

        builder.Property(ac => ac.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ac => ac.ClientId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ac => ac.UserId)
            .IsRequired();

        builder.Property(ac => ac.RedirectUri)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(ac => ac.Scope)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(ac => ac.ExpiresAt)
            .IsRequired();

        builder.HasIndex(ac => ac.Code)
            .IsUnique();
    }
}