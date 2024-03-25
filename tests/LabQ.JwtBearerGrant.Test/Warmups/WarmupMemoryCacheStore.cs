using LabQ.JwtBearerGrant.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LabQ.JwtBearerGrant.Test.Warmups;
public class WarmupMemoryCacheStore : IWarmupTest
{
    private readonly IAccessTokenStore _store;

    public WarmupMemoryCacheStore()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddJwtBearerGrant();
        serviceCollection.AddMemoryCache();

        Services = serviceCollection.BuildServiceProvider();

        _store = Services.GetRequiredService<IAccessTokenStore>();
    }

    public ServiceProvider Services { get; set; }

    public async Task Clear()
    {
        await _store.Clear();
    }
}
