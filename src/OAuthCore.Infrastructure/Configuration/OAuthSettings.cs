namespace OAuthCore.Infrastructure.Configuration;

public class OAuthCoreSettings
{
    public string POSTGRES_DB { get; set; } = string.Empty;
    public string POSTGRES_USER { get; set; } = string.Empty;
    public string POSTGRES_PASSWORD { get; set; } = string.Empty;
    public int POSTGRES_PORT { get; set; }
    public string DB_HOST { get; set; } = string.Empty;
    public string DB_CONNECTION_STRING { get; set; } = string.Empty;
    public string JWT_SECRET_KEY { get; set; } = string.Empty;
    public string JWT_ISSUER { get; set; } = string.Empty;
    public string JWT_AUDIENCE { get; set; } = string.Empty;
    public int ACCESS_TOKEN_EXPIRATION_SECONDS { get; set; } = 3600;
    public int REFRESH_TOKEN_EXPIRATION_SECONDS { get; set; }
    public int ID_TOKEN_EXPIRATION_SECONDS { get; set; }
    public int AUTH_CODE_EXPIRATION_SECONDS { get; set; } = 600;
}