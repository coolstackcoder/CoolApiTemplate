namespace OAuthCore.Application.DTOs;

public class TokenResponseDto
{
    public required string AccessToken { get; set; }
    public required string TokenType { get; set; }
    public required int ExpiresIn { get; set; }
    public string? RefreshToken { get; set; }
    public required string Scope { get; set; }
}