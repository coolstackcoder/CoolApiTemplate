using OAuthCore.Domain.Entities;

namespace OAuthCore.Application.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    // Add other methods as needed
}