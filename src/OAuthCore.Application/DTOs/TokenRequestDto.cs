namespace OAuthCore.Application.DTOs;

public class TokenRequestDto
{
    public required string GrantType { get; set; }
    public required string Code { get; set; }
    public required string RedirectUri { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}