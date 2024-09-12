using Microsoft.AspNetCore.Mvc;
using OAuthCore.Application.Interfaces;
using OAuthCore.Domain.Entities;
using Microsoft.AspNetCore.Http.Extensions;


namespace OAuthCore.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly IUserService _userService;
    private readonly IAuthorizationCodeService _authorizationCodeService;

    public AuthorizationController(
        IClientService clientService,
        IUserService userService,
        IAuthorizationCodeService authorizationCodeService)
    {
        _clientService = clientService;
        _userService = userService;
        _authorizationCodeService = authorizationCodeService;
    }

    [HttpGet("authorize")]
    public async Task<IActionResult> Authorize(
        [FromQuery] string response_type,
        [FromQuery] string client_id,
        [FromQuery] string redirect_uri,
        [FromQuery] string scope,
        [FromQuery] string state)
    {
        if (response_type != "code")
        {
            return BadRequest("Invalid response type");
        }

        var client = await _clientService.GetClientByIdAsync(client_id);
        if (client == null)
        {
            return BadRequest("Invalid client");
        }

        if (!client.RedirectUris.Contains(redirect_uri))
        {
            return BadRequest("Invalid redirect URI");
        }

        // For now, we'll use a hardcoded user ID. In a real application, this would come from user authentication.
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var authorizationCode = await _authorizationCodeService.CreateAuthorizationCodeAsync(client_id, userId, redirect_uri, scope);

        var query = new QueryBuilder();
        query.Add("code", authorizationCode.Code);
        if (!string.IsNullOrEmpty(state))
        {
            query.Add("state", state);
        }

        return Redirect($"{redirect_uri}{query}");
    }
}