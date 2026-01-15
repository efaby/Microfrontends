using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Shared.Auth;

public sealed class SharedAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private const string TokenKey = "mf_jwt";

    private readonly IJSRuntime _js;
    private ClaimsPrincipal _currentUser =
        new(new ClaimsIdentity());

    public SharedAuthenticationStateProvider(IJSRuntime js)
    {
        _js = js;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _js.InvokeAsync<string?>(
            "localStorage.getItem", TokenKey);

        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(_currentUser);

        SetPrincipal(token);

        return new AuthenticationState(_currentUser);
    }

    public async Task LoginWithJwtAsync(string jwt)
    {
        await _js.InvokeVoidAsync(
            "localStorage.setItem", TokenKey, jwt);

        SetPrincipal(jwt);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public async Task LogoutAsync()
    {
        await _js.InvokeVoidAsync(
            "localStorage.removeItem", TokenKey);

        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser)));
    }

    private void SetPrincipal(string jwt)
    {
        var parsedClaims = JwtParser.ParseClaims(jwt).ToList();

        var claims = new List<Claim>();

        foreach (var c in parsedClaims)
        {
            if (c.Type == "sub")
                claims.Add(new Claim(ClaimTypes.NameIdentifier, c.Value));

            else if (c.Type == "name")
                claims.Add(new Claim(ClaimTypes.Name, c.Value));

            else if (c.Type == "email")
                claims.Add(new Claim(ClaimTypes.Email, c.Value));

            else if (c.Type == "role")
                claims.Add(new Claim(ClaimTypes.Role, c.Value));
        }

        var identity = new ClaimsIdentity(claims, "jwt");
        _currentUser = new ClaimsPrincipal(identity);
    }
}
