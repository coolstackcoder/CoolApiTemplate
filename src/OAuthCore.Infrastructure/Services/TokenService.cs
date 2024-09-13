using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OAuthCore.Application.Interfaces;
using OAuthCore.Application.DTOs;
using OAuthCore.Application.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Web;
using OAuthCore.Domain.Enums;
using OAuthCore.Domain.Exceptions;
using OAuthCore.Infrastructure.Configuration;

namespace OAuthCore.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationCodeService _authorizationCodeService;
    private readonly OAuthCoreSettings _oauthCoreSettings;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IUnitOfWork unitOfWork, IAuthorizationCodeService authorizationCodeService, OAuthCoreSettings oauthCoreSettings, ILogger<TokenService> logger)
    {
        _unitOfWork = unitOfWork;
        _authorizationCodeService = authorizationCodeService;
        _oauthCoreSettings = oauthCoreSettings;
        _logger = logger;
    }

    public async Task<TokenResponseDto> GenerateTokensAsync(string authorizationCode, string clientId, string clientSecret, string redirectUri)
    {
        _logger.LogInformation("GenerateTokensAsync called with clientId: {ClientId}", clientId);

        var authCode = await _authorizationCodeService.GetAuthorizationCodeAsync(authorizationCode);
        if (authCode == null)
        {
            _logger.LogWarning("Invalid authorization code: {AuthCode}", authorizationCode);
            throw new InvalidGrantException("Invalid authorization code");
        }

        _logger.LogDebug("Authorization code found: {AuthCode}", authorizationCode);

        var client = await _unitOfWork.Clients.GetByClientIdAsync(clientId);
        if (client == null)
        {
            _logger.LogWarning("Client not found: {ClientId}", clientId);
            throw new InvalidClientException("Invalid client credentials");
        }

        _logger.LogDebug("Client found: {ClientId}", clientId);

        // Decode and trim the secrets
        var decodedStoredSecret = HttpUtility.UrlDecode(client.ClientSecret).Trim();
        var decodedProvidedSecret = HttpUtility.UrlDecode(clientSecret).Trim();

        _logger.LogDebug("Decoded stored secret (length: {StoredLength}): '{StoredSecret}'",
        decodedStoredSecret.Length, decodedStoredSecret);
        _logger.LogDebug("Decoded provided secret (length: {ProvidedLength}): '{ProvidedSecret}'",
            decodedProvidedSecret.Length, decodedProvidedSecret);

        if (decodedStoredSecret != decodedProvidedSecret)
        {
            _logger.LogWarning("Client secret mismatch for client: {ClientId}", clientId);
            throw new InvalidClientException("Invalid client credentials");
        }

        if (authCode.RedirectUri != redirectUri)
        {
            _logger.LogWarning("Redirect URI mismatch. AuthCode RedirectUri: {AuthCodeRedirectUri}, Provided RedirectUri: {ProvidedRedirectUri}",
                authCode.RedirectUri, redirectUri);
            throw new InvalidGrantException("Invalid redirect URI");
        }

        // Generate access token
        var accessToken = GenerateJwtToken(authCode.UserId, authCode.Scope, TokenType.AccessToken);

        // Generate refresh token (optional)
        var refreshToken = GenerateRefreshToken();

        // Invalidate the used authorization code
        await _authorizationCodeService.InvalidateAuthorizationCodeAsync(authorizationCode);

        _logger.LogInformation("Token generated successfully for client: {ClientId}", clientId);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = _oauthCoreSettings.AccessTokenExpirationSeconds,
            RefreshToken = refreshToken,
            Scope = authCode.Scope
        };
    }

    private string GenerateJwtToken(Guid userId, string scope, TokenType tokenType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_oauthCoreSettings.JwtSecretKey ??
            throw new InvalidOperationException("JWT Secret Key is not configured."));

        int expirationSeconds = tokenType switch
        {
            TokenType.AccessToken => _oauthCoreSettings.AccessTokenExpirationSeconds,
            TokenType.IdToken => _oauthCoreSettings.IdTokenExpirationSeconds,
            _ => throw new ArgumentException("Invalid token type")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("scope", scope)
            }),
            Expires = DateTime.UtcNow.AddSeconds(expirationSeconds),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _oauthCoreSettings.JwtIssuer,
            Audience = _oauthCoreSettings.JwtAudience
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}