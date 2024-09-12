using OAuthCore.Domain.Entities;
using OAuthCore.Application.DTOs;

namespace OAuthCore.Application.Interfaces;

public interface IClientService
{
    Task<ClientDto> RegisterClientAsync(ClientRegistrationDto registrationDto);
    Task<ClientDto?> GetClientByIdAsync(string clientId);
}