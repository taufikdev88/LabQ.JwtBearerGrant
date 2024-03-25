using LabQ.JwtBearerGrant.Interfaces;
using LabQ.JwtBearerGrant.Models;
using Microsoft.Extensions.Caching.Memory;

namespace LabQ.JwtBearerGrant.Stores;
public class MemoryCacheStore(
    IMemoryCache cache) : IAccessTokenStore
{
    private readonly IMemoryCache _cache = cache;
    private readonly string CacheFormat = "LABQ-JWTBEARERGRANT-{0}";

    public Task Clear()
    {
        return Task.CompletedTask;
    }

    public Task<JwtBearerToken?> Get(Guid id)
    {
        var token = _cache.Get<JwtBearerToken>(string.Format(CacheFormat, id));
        return Task.FromResult(token);
    }

    public Task<JwtBearerToken?> GetLatestFor(string subject)
    {
        var token = _cache.Get<JwtBearerToken>(string.Format(CacheFormat, subject));
        return Task.FromResult(token);
    }

    public Task Revoke(Guid id)
    {
        _cache.Remove(string.Format(CacheFormat, id));
        return Task.CompletedTask;
    }

    public Task Store(JwtBearerToken token)
    {
        if (token.Id == default)
            throw new InvalidOperationException("Cannot store token with empty id");

        if (string.IsNullOrWhiteSpace(token.Subject))
            throw new InvalidOperationException("Cannot store token with empty subject");

        if (string.IsNullOrWhiteSpace(token.AccessToken))
            throw new InvalidOperationException("Cannot store token with empty access token");

        if (token.ExpiredAt < DateTime.UtcNow)
            throw new InvalidOperationException("Expired token could not be stored");

        _cache.Set(string.Format(CacheFormat, token.Id), token, token.ExpiredAt);
        _cache.Set(string.Format(CacheFormat, token.Subject), token, token.ExpiredAt);

        return Task.CompletedTask;
    }
}
