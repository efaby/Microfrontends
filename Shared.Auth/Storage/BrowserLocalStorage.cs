using Microfrontends.Shared.Core.Auth;
using Microsoft.JSInterop;

namespace Shared.Auth.Storage;

public sealed class BrowserLocalStorage : ITokenStorage
{
    private const string TokenKey = "access_token";
    private readonly IJSRuntime _js;

    public BrowserLocalStorage(IJSRuntime js)
    {
        _js = js;
    }

    public async ValueTask<string?> GetAsync()
        => await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

    public async ValueTask SetAsync(string token)
        => await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);

    public async ValueTask RemoveAsync()
        => await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
}
