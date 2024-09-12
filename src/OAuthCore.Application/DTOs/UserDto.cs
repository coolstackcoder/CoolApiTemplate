namespace OAuthCore.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    // Note: We typically don't include sensitive information like PasswordHash in DTOs
}