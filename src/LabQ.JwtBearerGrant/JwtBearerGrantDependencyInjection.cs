using LabQ.JwtBearerGrant;
using LabQ.JwtBearerGrant.Interfaces;
using LabQ.JwtBearerGrant.Services;
using LabQ.JwtBearerGrant.Stores;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Dependency injection class for integrate with asp net core
/// </summary>
public static class JwtBearerGrantDependencyInjection
{
    /// <summary>
    /// Add default services to DI Container
    /// </summary>
    /// <param name="services"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IJwtBearerGrantBuilder AddJwtBearerGrant(this IServiceCollection services, Action<JwtBearerGrantOptions>? action = null)
    {
        if (action != null)
            services.Configure(action);

        services.TryAddTransient<IAccessTokenService, AccessTokenService>();
        services.TryAddTransient<IAccessTokenStore, MemoryCacheStore>();
        services.TryAddTransient<IRSAFactory, DefaultRSAFactory>();

        return new JwtBearerGrantBuilder(services);
    }
}
