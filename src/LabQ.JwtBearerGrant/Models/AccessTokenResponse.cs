﻿using System.Text.Json.Serialization;

namespace LabQ.JwtBearerGrant.Models;
internal class AccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }
}
