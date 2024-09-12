namespace OAuthCore.Application.DTOs;

public class ClientDto
{
    public required string ClientId { get; set; }
    public required string Name { get; set; }
    public required string[] RedirectUris { get; set; }
    public required string[] AllowedGrantTypes { get; set; }
    public required string[] AllowedScopes { get; set; }
    // Note: We typically don't include sensitive information like ClientSecret in DTOs
}