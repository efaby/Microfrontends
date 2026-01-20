namespace Microfrontends.Shared.Core.Primitives;

public readonly record struct Result(bool IsSuccess, string? Error)
{
    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}
