using Microsoft.Extensions.DependencyInjection;

namespace LabQ.JwtBearerGrant.Test.Warmups;
public interface IWarmupTest
{
    ServiceProvider Services { get; set; }
    Task Clear();
}
