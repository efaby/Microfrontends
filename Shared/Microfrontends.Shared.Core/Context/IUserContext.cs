namespace Microfrontends.Shared.Core.Context;

public interface IUserContext
{
    string? UserId { get; }
    string? Email { get; }
    string? UserName { get; }
}
