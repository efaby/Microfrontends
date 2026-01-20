using System;

namespace Microfrontends.Shared.Core.Context;

public interface IUser
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Subject { get; init; }
}

