using OAuthCore.Domain.Entities;
using OAuthCore.Application.DTOs;

namespace OAuthCore.Application.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User> CreateUserAsync(UserRegistrationDto registrationDto);
}