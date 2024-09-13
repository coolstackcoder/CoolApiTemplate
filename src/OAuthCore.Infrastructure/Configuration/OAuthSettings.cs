namespace OAuthCore.Infrastructure.Configuration;
public class OAuthCoreSettings
{
    public string PostgresDb { get; set; } = string.Empty;
    public string PostgresUser { get; set; } = string.Empty;
    public string PostgresPassword { get; set; } = string.Empty;
    public int PostgresPort { get; set; }
    public string DbHost { get; set; } = string.Empty;
    public string DbConnectionString { get; set; } = string.Empty;
    public string JwtSecretKey { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = string.Empty;
    public string JwtAudience { get; set; } = string.Empty;
    public int AccessTokenExpirationSeconds { get; set; } = 3600;
    public int RefreshTokenExpirationSeconds { get; set; }
    public int IdTokenExpirationSeconds { get; set; }
    public int AuthCodeExpirationSeconds { get; set; } = 600;
}