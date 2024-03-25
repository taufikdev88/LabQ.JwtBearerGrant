namespace LabQ.JwtBearerGrant.Models;
public class JwtBearerToken
{
    public Guid Id { get; set; }
    public string? Subject { get; set; }
    public string? AccessToken { get; set; }
    public string? Scope { get; set; }
    public string? TokenType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiredAt { get; set; }
}
