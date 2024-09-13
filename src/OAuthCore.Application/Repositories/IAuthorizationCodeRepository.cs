using OAuthCore.Domain.Entities;

namespace OAuthCore.Application.Repositories;

public interface IAuthorizationCodeRepository
{
    Task<AuthorizationCode?> GetByCodeAsync(string code);
    Task<AuthorizationCode> CreateAsync(AuthorizationCode authorizationCode);
    Task RemoveAsync(AuthorizationCode authorizationCode);
    // Add other methods as needed
}