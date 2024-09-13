using Microsoft.EntityFrameworkCore;
using OAuthCore.Domain.Entities;

namespace OAuthCore.Application.Data;

public interface IDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Client> Clients { get; set; }
    DbSet<AuthorizationCode> AuthorizationCodes { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}