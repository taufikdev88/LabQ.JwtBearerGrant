using LabQ.JwtBearerGrant.Models;

namespace LabQ.JwtBearerGrant.Interfaces;
public interface IAccessTokenStore
{
    Task Store(JwtBearerToken token);
    Task<JwtBearerToken?> Get(Guid id);
    Task<JwtBearerToken?> GetLatestFor(string subject);
    Task Revoke(Guid id);
    Task Clear();
}
