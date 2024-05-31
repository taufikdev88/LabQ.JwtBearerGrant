using LabQ.JwtBearerGrant;
using LabQ.JwtBearerGrant.Interfaces;
using LabQ.JwtBearerGrant.Services;
using LabQ.JwtBearerGrant.Stores;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class JwtBearerGrantDependencyInjection
{
    public static IJwtBearerGrantBuilder AddJwtBearerGrant(this IServiceCollection services, Action<JwtBearerGrantOptions>? action = null)
    {
        if (action != null)
            services.Configure(action);

        services.TryAddScoped<IAccessTokenService, AccessTokenService>();
        services.TryAddScoped<IAccessTokenStore, MemoryCacheStore>();
        services.TryAddScoped<IRSAFactory, DefaultRSAFactory>();

        return new JwtBearerGrantBuilder(services);
    }
}
