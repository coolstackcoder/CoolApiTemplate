using Microsoft.AspNetCore.Mvc;
using OAuthCore.Application.Interfaces;
using OAuthCore.Application.DTOs;
using Microsoft.Extensions.Logging;

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
        _logger.LogInformation("GenerateToken method called");

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

            _logger.LogInformation("Token generated successfully");
            return Ok(tokenResponse);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error generating token");
            return BadRequest(new { error = "invalid_request", error_description = ex.Message });
        }
    }
}