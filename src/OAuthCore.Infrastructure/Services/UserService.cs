using OAuthCore.Application.Interfaces;
using OAuthCore.Domain.Entities;
using OAuthCore.Application.DTOs;
using OAuthCore.Application.Repositories;

namespace OAuthCore.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(UserRegistrationDto registrationDto)
    {
        var user = new User
        {
            Username = registrationDto.Username,
            Email = registrationDto.Email,
            PasswordHash = registrationDto.Password // This should be hashed in a real application
        };

        await _unitOfWork.Users.CreateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(user);
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }
}