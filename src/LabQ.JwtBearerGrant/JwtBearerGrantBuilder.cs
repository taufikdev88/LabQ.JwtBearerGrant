using LabQ.JwtBearerGrant.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LabQ.JwtBearerGrant;
public class JwtBearerGrantBuilder(IServiceCollection services) : IJwtBearerGrantBuilder
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));
}
