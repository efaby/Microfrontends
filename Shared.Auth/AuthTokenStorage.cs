using System.Text.Json;
using Microsoft.JSInterop;

namespace Shared.Auth;

public sealed class AuthTokenStorage
{
    private const string Key = "shared-auth";

    private readonly IJSRuntime _js;

    public AuthTokenStorage(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SaveAsync(UserInfo user)
    {
        var json = JsonSerializer.Serialize(user);
        await _js.InvokeVoidAsync("localStorage.setItem", Key, json);
    }

    public async Task<UserInfo?> LoadAsync()
    {
        var json = await _js.InvokeAsync<string?>(
            "localStorage.getItem", Key);

        return json is null
            ? null
            : JsonSerializer.Deserialize<UserInfo>(json);
    }

    public async Task ClearAsync()
    {
        await _js.InvokeVoidAsync(
            "localStorage.removeItem", Key);
    }
}
