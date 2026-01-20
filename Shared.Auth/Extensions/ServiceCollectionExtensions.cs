using Microsoft.Extensions.DependencyInjection;
using Shared.Auth.Storage;
using Microfrontends.Shared.Core.Auth;
using Microfrontends.Shared.Auth.Services;
using Microsoft.Extensions.Configuration;

namespace Shared.Auth.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedAuth(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, LocalStorageAuthService>();
        services.AddScoped<ITokenStorage, BrowserLocalStorage>();

        return services;
    }

    public static IServiceCollection AddMicrofrontendCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // registros
        return services;
    }
}
