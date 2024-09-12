using OAuthCore.Domain.Entities;
using OAuthCore.Application.DTOs;

namespace OAuthCore.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto> CreateUserAsync(UserRegistrationDto registrationDto);
}