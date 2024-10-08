using Microsoft.EntityFrameworkCore;
using OAuthCore.Application.Data;
using OAuthCore.Application.Repositories;
using OAuthCore.Domain.Entities;
using OAuthCore.Infrastructure.Data;

namespace OAuthCore.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly IDbContext _context;

    public ClientRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetByClientIdAsync(string clientId)
    {
        return await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
    }

    public async Task<Client> CreateAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
        return client;
    }

    // Implement other methods as needed
}