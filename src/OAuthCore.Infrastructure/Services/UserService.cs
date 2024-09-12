using OAuthCore.Application.Interfaces;
using OAuthCore.Domain.Entities;
using OAuthCore.Infrastructure.Data;
using OAuthCore.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace OAuthCore.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly OAuthDbContext _context;

    public UserService(OAuthDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
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

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
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