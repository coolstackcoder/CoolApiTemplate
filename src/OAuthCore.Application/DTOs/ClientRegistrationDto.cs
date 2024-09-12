namespace OAuthCore.Application.DTOs;

public class ClientRegistrationDto
{
    public required string Name { get; set; }
    public required string[] RedirectUris { get; set; }
    public required string[] AllowedGrantTypes { get; set; }
    public required string[] AllowedScopes { get; set; }
}