using Microsoft.Extensions.DependencyInjection;

namespace LabQ.JwtBearerGrant.Interfaces;
public interface IJwtBearerGrantBuilder
{
    IServiceCollection Services { get; }
}
