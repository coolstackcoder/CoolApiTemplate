using OAuthCore.Application.Interfaces;
using OAuthCore.Domain.Entities;
using OAuthCore.Application.DTOs;
using OAuthCore.Application.Repositories;
using OAuthCore.Infrastructure.Configuration;

namespace OAuthCore.Infrastructure.Services;

public class AuthorizationCodeService : IAuthorizationCodeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OAuthCoreSettings _oauthcoreSettings;

    public AuthorizationCodeService(IUnitOfWork unitOfWork, OAuthCoreSettings oauthcoreSettings)
    {
        _unitOfWork = unitOfWork;
        _oauthcoreSettings = oauthcoreSettings;
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
            ExpiresAt = DateTime.UtcNow.AddSeconds(_oauthcoreSettings.AUTH_CODE_EXPIRATION_SECONDS)
        };

        await _unitOfWork.AuthorizationCodes.CreateAsync(authorizationCode);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(authorizationCode);
    }

    public async Task<AuthorizationCodeDto?> GetAuthorizationCodeAsync(string code)
    {
        var authorizationCode = await _unitOfWork.AuthorizationCodes.GetByCodeAsync(code);
        return authorizationCode != null && authorizationCode.ExpiresAt > DateTime.UtcNow ? MapToDto(authorizationCode) : null;
    }

    public async Task InvalidateAuthorizationCodeAsync(string code)
    {
        var authorizationCode = await _unitOfWork.AuthorizationCodes.GetByCodeAsync(code);
        if (authorizationCode != null)
        {
            await _unitOfWork.AuthorizationCodes.RemoveAsync(authorizationCode);
            await _unitOfWork.SaveChangesAsync();
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