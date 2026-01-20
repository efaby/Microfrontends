using System;

namespace Microfrontends.Shared.Core.Auth;

using Microfrontends.Shared.Core.Context;

public interface IAuthClient
{
    Task<UserDto?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
}

