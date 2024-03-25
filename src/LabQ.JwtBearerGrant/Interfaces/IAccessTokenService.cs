namespace LabQ.JwtBearerGrant.Interfaces;
public interface IAccessTokenService
{
    Task<string> GetAccessTokenFor(string subject, IEnumerable<string> scopes);
}
