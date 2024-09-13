using OAuthCore.Application.Repositories;
using OAuthCore.Infrastructure.Data;

namespace OAuthCore.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly OAuthDbContext _context;

    public IUserRepository Users { get; }
    public IClientRepository Clients { get; }
    public IAuthorizationCodeRepository AuthorizationCodes { get; }

    public UnitOfWork(OAuthDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Clients = new ClientRepository(_context);
        AuthorizationCodes = new AuthorizationCodeRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}