using OAuthCore.Domain.Entities;
using OAuthCore.Application.DTOs;

namespace OAuthCore.Application.Interfaces;

public interface IClientService
{
    Task<Client> RegisterClientAsync(ClientRegistrationDto registrationDto);
    Task<Client?> GetClientByIdAsync(string clientId);
}