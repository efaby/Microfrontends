namespace Microfrontends.Shared.Core.Auth;

public interface ITokenStorage
{
    ValueTask<string?> GetAsync();
    ValueTask SetAsync(string token);
    ValueTask RemoveAsync();
}
