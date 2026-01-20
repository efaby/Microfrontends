using System;
using Microsoft.Extensions.DependencyInjection;
using Microfrontends.Shared.Core.Auth;

namespace Shared.Auth.AuthCognito;

public static class AuthExtensions
{

    public static IServiceCollection AddSharedAuth(
            this IServiceCollection services)
    {
        services.AddScoped<IAuthClient, AuthClient>();
        return services;
    }
}
