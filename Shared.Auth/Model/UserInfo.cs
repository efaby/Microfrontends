using System;
using Microfrontends.Shared.Core.Context;

namespace Microfrontends.Shared.Core.Auth;

public class UserInfo : IUserContext
{
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
