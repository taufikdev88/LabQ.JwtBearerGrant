namespace LabQ.JwtBearerGrant;
public class JwtBearerGrantOptions
{
    public string CertDir { get; set; } = $"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "certs")}";
    public string CertName { get; set; } = "private.pem";
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }

    public string GetFullPath() => Path.Combine(CertDir, CertName);
}
