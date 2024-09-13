namespace OAuthCore.Application.Repositories;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository Users { get; }
    IClientRepository Clients { get; }
    IAuthorizationCodeRepository AuthorizationCodes { get; }
    Task<int> SaveChangesAsync();
}