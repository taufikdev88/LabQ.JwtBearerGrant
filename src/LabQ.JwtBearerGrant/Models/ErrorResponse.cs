using System.Text.Json.Serialization;

namespace LabQ.JwtBearerGrant.Models;
internal class ErrorResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; set; }
    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; set; }
}
