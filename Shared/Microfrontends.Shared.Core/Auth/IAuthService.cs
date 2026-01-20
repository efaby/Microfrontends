using Microfrontends.Shared.Core.Context;

namespace Microfrontends.Shared.Core.Auth;

public interface IAuthService
{
    Task<string?> GetAccessTokenAsync();
    Task<bool> IsAuthenticatedAsync();
    Task LogoutAsync();
    Task SetAccessTokenAsync(string token);
    Task<IUserContext> GetUserAsync();

}
