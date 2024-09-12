using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OAuthCore.Application.Interfaces;
using OAuthCore.Application.DTOs;
using OAuthCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Web;
using OAuthCore.Domain.Enums;
using OAuthCore.Domain.Exceptions;

namespace OAuthCore.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly OAuthDbContext _context;
    private readonly IAuthorizationCodeService _authorizationCodeService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;

    public TokenService(OAuthDbContext context, IAuthorizationCodeService authorizationCodeService, IConfiguration configuration, ILogger<TokenService> logger)
    {
        _context = context;
        _authorizationCodeService = authorizationCodeService;
        _configuration = configuration;
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

        var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
        if (client == null)
        {
            _logger.LogWarning("Client not found: {ClientId}", clientId);
            throw new InvalidOperationException("Invalid client credentials");
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
            throw new InvalidOperationException("Invalid client credentials");
        }

        if (authCode.RedirectUri != redirectUri)
        {
            _logger.LogWarning("Redirect URI mismatch. AuthCode RedirectUri: {AuthCodeRedirectUri}, Provided RedirectUri: {ProvidedRedirectUri}",
                authCode.RedirectUri, redirectUri);
            throw new InvalidOperationException("Invalid redirect URI");
        }

        // Generate access token
        var accessToken = GenerateJwtToken(authCode.UserId, authCode.Scope, TokenType.AccessToken);

        // Generate refresh token (optional)
        var refreshToken = GenerateRefreshToken();

        // Invalidate the used authorization code
        await _authorizationCodeService.InvalidateAuthorizationCodeAsync(authorizationCode);

        _logger.LogInformation("Token generated successfully for client: {ClientId}", clientId);

        _logger.LogDebug("Get from .env file ACCESS_TOKEN_EXPIRATION_SECONDS: {ACCESS_TOKEN_EXPIRATION_SECONDS}", Environment.GetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_SECONDS"));

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = int.Parse(Environment.GetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_SECONDS") ?? "3600"),
            RefreshToken = refreshToken,
            Scope = authCode.Scope
        };
    }

    private string GenerateJwtToken(Guid userId, string scope, TokenType tokenType)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ??
            throw new InvalidOperationException("JWT Secret Key is not configured."));

        int expirationSeconds = tokenType switch
        {
            TokenType.AccessToken => int.Parse(Environment.GetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_SECONDS") ?? "3600"),
            TokenType.IdToken => int.Parse(Environment.GetEnvironmentVariable("ID_TOKEN_EXPIRATION_SECONDS") ?? "3600"),
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
            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}