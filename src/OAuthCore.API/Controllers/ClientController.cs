using Microsoft.AspNetCore.Mvc;
using OAuthCore.Application.Interfaces;
using OAuthCore.Application.DTOs;

namespace OAuthCore.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterClient(ClientRegistrationDto registrationDto)
    {
        var registeredClient = await _clientService.RegisterClientAsync(registrationDto);
        return CreatedAtAction(nameof(GetClient), new { clientId = registeredClient.ClientId }, registeredClient);
    }

    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetClient(string clientId)
    {
        var client = await _clientService.GetClientByIdAsync(clientId);
        if (client == null)
        {
            return NotFound();
        }
        return Ok(client);
    }
}