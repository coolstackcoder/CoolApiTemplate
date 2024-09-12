namespace OAuthCore.Application.DTOs;

public class AuthorizationCodeDto
{
    public required string Code { get; set; }
    public required string ClientId { get; set; }
    public required Guid UserId { get; set; }
    public required string RedirectUri { get; set; }
    public required string Scope { get; set; }
    public required DateTime ExpiresAt { get; set; }
}