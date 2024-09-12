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

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> CreateUserAsync(UserRegistrationDto registrationDto)
    {
        // In a real application, you would hash the password here
        var user = new User
        {
            Username = registrationDto.Username,
            Email = registrationDto.Email,
            PasswordHash = registrationDto.Password // This should be hashed in a real application
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}