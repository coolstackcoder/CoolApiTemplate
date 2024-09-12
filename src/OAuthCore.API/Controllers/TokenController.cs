using Microsoft.AspNetCore.Mvc;
using OAuthCore.Application.Interfaces;
using Microsoft.Extensions.Logging;
using OAuthCore.Domain.Exceptions;
using System.Text;
using OAuthCore.Application.DTOs;

namespace OAuthCore.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<TokenController> _logger;

    public TokenController(ITokenService tokenService, ILogger<TokenController> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
        _logger.LogInformation("TokenController constructed");
    }

    [HttpPost]
    public async Task<IActionResult> GenerateToken()
    {
        _logger.LogInformation("Token generation requested");

        // Log all form data
        foreach (var key in Request.Form.Keys)
        {
            _logger.LogInformation($"Form data: {key} = {Request.Form[key]}");
        }

        string? rawclientSecret = Request.Form["client_secret"];

        // Log the client secret safely, replacing null with an empty string or some placeholder
        _logger.LogDebug("Raw client secret: '{RawSecret}'", rawclientSecret ?? "null");

        // Manually extract form data
        var grantType = Request.Form["grant_type"].ToString();
        var code = Request.Form["code"].ToString();
        var redirectUri = Request.Form["redirect_uri"].ToString();
        var clientId = Request.Form["client_id"].ToString();
        var clientSecret = Request.Form["client_secret"].ToString();

        _logger.LogInformation($"Extracted grant_type: {grantType}");

        if (string.IsNullOrEmpty(grantType) || grantType != "authorization_code")
        {
            _logger.LogWarning("Unsupported grant type: {GrantType}", grantType);
            return BadRequest(new { error = "unsupported_grant_type" });
        }

        try
        {
            var tokenResponse = await _tokenService.GenerateTokensAsync(
                code,
                clientId,
                clientSecret,
                redirectUri
            );

            _logger.LogInformation("Token generated successfully for client {ClientId}", clientId);
            return Ok(tokenResponse);
        }
        catch (OAuthException ex)
        {
            _logger.LogWarning(ex, "OAuth error occurred during token generation");
            return StatusCode(ex.StatusCode, new ErrorResponseDto(
                type: "https://tools.ietf.org/html/rfc6749#section-5.2",
                title: ToSnakeCase(ex.GetType().Name.Replace("Exception", "")),
                status: ex.StatusCode,
                detail: ex.Message,
                instance: HttpContext.Request.Path
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during token generation");
            throw; // Let the middleware handle unexpected exceptions
        }
    }

    private string ToSnakeCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var builder = new StringBuilder(text.Length + Math.Min(2, text.Length / 5));
        var previousCategory = char.GetUnicodeCategory(text[0]);

        for (int currentIndex = 0; currentIndex < text.Length; currentIndex++)
        {
            var currentChar = text[currentIndex];
            var currentCategory = char.GetUnicodeCategory(currentChar);

            if (currentIndex > 0 &&
                (currentCategory == System.Globalization.UnicodeCategory.UppercaseLetter ||
                 currentCategory == System.Globalization.UnicodeCategory.TitlecaseLetter) &&
                previousCategory != System.Globalization.UnicodeCategory.SpaceSeparator)
            {
                builder.Append('_');
            }

            builder.Append(char.ToLower(currentChar));

            previousCategory = currentCategory;
        }

        return builder.ToString();
    }
}