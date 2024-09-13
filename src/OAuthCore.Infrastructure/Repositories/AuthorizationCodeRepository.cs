using Microsoft.EntityFrameworkCore;
using OAuthCore.Application.Data;
using OAuthCore.Application.Repositories;
using OAuthCore.Domain.Entities;
using OAuthCore.Infrastructure.Data;

namespace OAuthCore.Infrastructure.Repositories;

public class AuthorizationCodeRepository : IAuthorizationCodeRepository
{
    private readonly IDbContext _context;

    public AuthorizationCodeRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<AuthorizationCode?> GetByCodeAsync(string code)
    {
        return await _context.AuthorizationCodes.FirstOrDefaultAsync(ac => ac.Code == code);
    }

    public async Task<AuthorizationCode> CreateAsync(AuthorizationCode authorizationCode)
    {
        await _context.AuthorizationCodes.AddAsync(authorizationCode);
        return authorizationCode;
    }

    public Task RemoveAsync(AuthorizationCode authorizationCode)
    {
        _context.AuthorizationCodes.Remove(authorizationCode);
        return Task.CompletedTask;
    }

    // Implement other methods as needed
}