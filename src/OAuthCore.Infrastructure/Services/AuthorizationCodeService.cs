using OAuthCore.Application.Interfaces;
using OAuthCore.Domain.Entities;
using OAuthCore.Infrastructure.Data;
using OAuthCore.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace OAuthCore.Infrastructure.Services;

public class AuthorizationCodeService : IAuthorizationCodeService
{
    private readonly OAuthDbContext _context;

    public AuthorizationCodeService(OAuthDbContext context)
    {
        _context = context;
    }

    public async Task<AuthorizationCodeDto> CreateAuthorizationCodeAsync(string clientId, Guid userId, string redirectUri, string scope)
    {
        var authorizationCode = new AuthorizationCode
        {
            Code = GenerateAuthorizationCode(),
            ClientId = clientId,
            UserId = userId,
            RedirectUri = redirectUri,
            Scope = scope,
            ExpiresAt = DateTime.UtcNow.AddSeconds(int.Parse(Environment.GetEnvironmentVariable("AUTH_CODE_EXPIRATION_SECONDS") ?? "600"))
        };

        _context.AuthorizationCodes.Add(authorizationCode);
        await _context.SaveChangesAsync();

        return MapToDto(authorizationCode);
    }

    public async Task<AuthorizationCodeDto?> GetAuthorizationCodeAsync(string code)
    {
        var authorizationCode = await _context.AuthorizationCodes
            .FirstOrDefaultAsync(ac => ac.Code == code && ac.ExpiresAt > DateTime.UtcNow);

        return authorizationCode != null ? MapToDto(authorizationCode) : null;
    }

    public async Task InvalidateAuthorizationCodeAsync(string code)
    {
        var authorizationCode = await _context.AuthorizationCodes.FirstOrDefaultAsync(ac => ac.Code == code);
        if (authorizationCode != null)
        {
            _context.AuthorizationCodes.Remove(authorizationCode);
            await _context.SaveChangesAsync();
        }
    }

    private string GenerateAuthorizationCode()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    private AuthorizationCodeDto MapToDto(AuthorizationCode authorizationCode)
    {
        return new AuthorizationCodeDto
        {
            Code = authorizationCode.Code,
            ClientId = authorizationCode.ClientId,
            UserId = authorizationCode.UserId,
            RedirectUri = authorizationCode.RedirectUri,
            Scope = authorizationCode.Scope,
            ExpiresAt = authorizationCode.ExpiresAt
        };
    }
}