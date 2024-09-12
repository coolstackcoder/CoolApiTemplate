using OAuthCore.Application.DTOs;

namespace OAuthCore.Application.Interfaces;

public interface IAuthorizationCodeService
{
    Task<AuthorizationCodeDto> CreateAuthorizationCodeAsync(string clientId, Guid userId, string redirectUri, string scope);
    Task<AuthorizationCodeDto?> GetAuthorizationCodeAsync(string code);
    Task InvalidateAuthorizationCodeAsync(string code);
}