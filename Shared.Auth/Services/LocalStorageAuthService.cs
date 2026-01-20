using Microfrontends.Shared.Core.Auth;
using System.Security.Claims;
using Shared.Auth.Storage;
using Shared.Auth;
using Microfrontends.Shared.Core.Context;

namespace Microfrontends.Shared.Auth.Services;

public sealed class LocalStorageAuthService : IAuthService
{
    private readonly ITokenStorage _tokenStorage;

    public LocalStorageAuthService(ITokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await _tokenStorage.GetAsync();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _tokenStorage.GetAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task LogoutAsync()
    {
        await _tokenStorage.RemoveAsync();
    }

    public async Task SetAccessTokenAsync(string token)
    {
        await _tokenStorage.SetAsync(token);
    }

    public async Task<IUserContext> GetUserAsync()
    {
        var token = await _tokenStorage.GetAsync();
        if (string.IsNullOrWhiteSpace(token))
            return new UserInfo();

        return SetPrincipal(token);
    }

    private UserInfo SetPrincipal(string jwt)
    {
        var parsedClaims = JwtParser.ParseClaims(jwt).ToList();

        var claims = new List<Claim>();

        var user = new UserInfo
        {
            UserId = parsedClaims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty,
            UserName = parsedClaims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty,
            Email = parsedClaims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty
        };

        return user;
    }
}
