using OAuthCore.Application.Interfaces;
using OAuthCore.Domain.Entities;
using OAuthCore.Application.DTOs;
using OAuthCore.Application.Repositories;

namespace OAuthCore.Infrastructure.Services;

public class ClientService : IClientService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClientService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.Clients.CreateAsync(client);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(client);
    }

    public async Task<ClientDto?> GetClientByIdAsync(string clientId)
    {
        var client = await _unitOfWork.Clients.GetByClientIdAsync(clientId);
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