using OAuthCore.Application.DTOs;

namespace OAuthCore.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResponseDto> GenerateTokensAsync(string authorizationCode, string clientId, string clientSecret, string redirectUri);
}