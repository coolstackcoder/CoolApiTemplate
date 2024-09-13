using Microsoft.EntityFrameworkCore;
using OAuthCore.Application.Repositories;
using OAuthCore.Domain.Entities;
using OAuthCore.Infrastructure.Data;

namespace OAuthCore.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly OAuthDbContext _context;

    public UserRepository(OAuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        return user;
    }

    // Implement other methods as needed
}