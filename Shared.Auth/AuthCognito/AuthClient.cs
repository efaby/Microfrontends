using System;
using System.Net.Http.Json;
using Microfrontends.Shared.Core.Auth;
using Microfrontends.Shared.Core.Context;

namespace Shared.Auth.AuthCognito;

public class AuthClient : IAuthClient
{
    private readonly HttpClient _http;
    private UserDto? _cachedUser;

    public AuthClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        if (_cachedUser != null)
            return _cachedUser;

        var res = await _http.GetAsync("auth/me");

        if (!res.IsSuccessStatusCode)
            return null;

        _cachedUser = await res.Content.ReadFromJsonAsync<UserDto>();
        return _cachedUser;
    }

    public async Task<bool> IsAuthenticatedAsync()
        => (await GetCurrentUserAsync()) != null;
}
