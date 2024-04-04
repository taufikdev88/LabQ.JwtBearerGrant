using LabQ.JwtBearerGrant.Models;

namespace LabQ.JwtBearerGrant.Interfaces;
public interface IAccessTokenService
{
    Task<string> GetAccessTokenFor(string subject, IEnumerable<string> scopes);
    Task<JwtBearerToken> GetTokenFor(string subject, IEnumerable<string> scopes);
}
