using OAuthCore.Application.Interfaces;
using OAuthCore.Domain.Entities;
using OAuthCore.Infrastructure.Data;
using OAuthCore.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace OAuthCore.Infrastructure.Services;

public class ClientService : IClientService
{
    private readonly OAuthDbContext _context;

    public ClientService(OAuthDbContext context)
    {
        _context = context;
    }

    public async Task<ClientDto> RegisterClientAsync(ClientRegistrationDto registrationDto)
    {
        var client = new Client
        {
            ClientId = Guid.NewGuid().ToString("N"),
            ClientSecret = GenerateClientSecret(),
            Name = registrationDto.Name,
            RedirectUris = registrationDto.RedirectUris,
            AllowedGrantTypes = registrationDto.AllowedGrantTypes,
            AllowedScopes = registrationDto.AllowedScopes
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return MapToDto(client);
    }

    public async Task<ClientDto?> GetClientByIdAsync(string clientId)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
        return client != null ? MapToDto(client) : null;
    }

    private string GenerateClientSecret()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    private ClientDto MapToDto(Client client)
    {
        return new ClientDto
        {
            ClientId = client.ClientId,
            Name = client.Name,
            RedirectUris = client.RedirectUris,
            AllowedGrantTypes = client.AllowedGrantTypes,
            AllowedScopes = client.AllowedScopes
        };
    }
}