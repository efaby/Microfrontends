using System;
using System.Net.Http.Json;

namespace Shared.Auth.Services;

public record UserInfo(string? Name, string? Email);
public class AuthService
{

    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<UserInfo?> GetMeAsync()
    {
        var res = await _http.GetAsync("auth/me");

        if (!res.IsSuccessStatusCode)
            return null;

        return await res.Content.ReadFromJsonAsync<UserInfo>();
    }

    public async Task<bool> IsAuthenticatedAsync()
        => (await GetMeAsync()) != null;
}
