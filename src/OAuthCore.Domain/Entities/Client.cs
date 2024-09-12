namespace OAuthCore.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string Name { get; set; }
    public required string[] RedirectUris { get; set; }
    public required string[] AllowedGrantTypes { get; set; }
    public required string[] AllowedScopes { get; set; }
}