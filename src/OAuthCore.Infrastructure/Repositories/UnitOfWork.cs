using OAuthCore.Application.Data;
using OAuthCore.Application.Repositories;

namespace OAuthCore.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext _context;

    public IUserRepository Users { get; }
    public IClientRepository Clients { get; }
    public IAuthorizationCodeRepository AuthorizationCodes { get; }

    public UnitOfWork(IDbContext context)
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
        if (_context is IAsyncDisposable disposableContext)
        {
            await disposableContext.DisposeAsync();
        }
    }
}