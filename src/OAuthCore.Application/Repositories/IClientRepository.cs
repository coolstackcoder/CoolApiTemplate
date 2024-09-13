using OAuthCore.Domain.Entities;

namespace OAuthCore.Application.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByClientIdAsync(string clientId);
    Task<Client> CreateAsync(Client client);
    // Add other methods as needed
}