using LabQ.JwtBearerGrant.Interfaces;
using LabQ.JwtBearerGrant.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LabQ.JwtBearerGrant.Services;
public class AccessTokenService(
    IAccessTokenStore accessTokenStore,
    IOptions<JwtBearerGrantOptions> options,
    IHttpClientFactory httpClientFactory) : IAccessTokenService
{
    private readonly IAccessTokenStore _accessTokenStore = accessTokenStore;
    private readonly IOptions<JwtBearerGrantOptions> _options = options;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<JwtBearerToken> GetTokenFor(string subject, IEnumerable<string> scopes)
    {
        var token = await _accessTokenStore.GetLatestFor(subject);
        if (token != null && token.ExpiredAt < DateTime.UtcNow)
            return token!;

        var tokenId = Guid.NewGuid();

        var jwtAssertion = await GenerateAssertion(tokenId, subject, scopes);
        var jwtToken = await ExchangeWithToken(jwtAssertion, scopes);

        token = new JwtBearerToken
        {
            Id = tokenId,
            Subject = subject,
            AccessToken = jwtToken.AccessToken,
            Scope = jwtToken.Scope,
            TokenType = jwtToken.TokenType,
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMilliseconds(jwtToken.ExpiresIn)
        };

        return token;
    }

    public async Task<string> GetAccessTokenFor(string subject, IEnumerable<string> scopes)
    {
        var token = await GetTokenFor(subject, scopes);

        return token.AccessToken!;
    }

    private async Task<string> GenerateAssertion(Guid id, string subject, IEnumerable<string> scopes)
    {
        // read private key
        var certificate = await File.ReadAllTextAsync(_options.Value.GetFullPath());

        // read as rsa
        using var rsa = RSA.Create();
        rsa.ImportFromPem(certificate);

        // building header
        var securityKey = new RsaSecurityKey(rsa);
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        var jwtHeader = new JwtHeader(signingCredentials);

        // building payload
        var claims = new List<Claim>(scopes.Count() + 2)
        {
            new("sub", subject),
            new("jti", id.ToString())
        };
        claims.AddRange(scopes.Select(s => new Claim("scp", s)));

        var jwtPayload = new JwtPayload(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            notBefore: null,
            expires: DateTime.UtcNow.AddMinutes(10),
            issuedAt: DateTime.UtcNow,
            claims: claims);

        // generate token
        var token = new JwtSecurityToken(jwtHeader, jwtPayload);
        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);
    }

    private async Task<AccessTokenResponse> ExchangeWithToken(string assertion, IEnumerable<string> scopes)
    {
        var client = _httpClientFactory.CreateClient();

        var content = new FormUrlEncodedContent([
            new("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
            new("scope", string.Join(' ', scopes)),
            new("assertion", assertion)]);

        var tokenRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_options.Value.Audience),
            Content = content
        };

        var authenticationString = $"{_options.Value.ClientId}:{_options.Value.ClientSecret}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

        tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

        var tokenResponse = await client.SendAsync(tokenRequest);
        var tokenResponseAsString = await tokenResponse.Content.ReadAsStringAsync();

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(tokenResponseAsString);
            throw new HttpRequestException($"{error?.Error}: {error?.ErrorDescription}");
        }

        var result = JsonSerializer.Deserialize<AccessTokenResponse>(tokenResponseAsString)
            ?? throw new InvalidCastException("Cannot deserialize as Access Token Response");

        return result;
    }
}
