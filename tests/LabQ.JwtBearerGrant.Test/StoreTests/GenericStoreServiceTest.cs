using Bogus;
using FluentAssertions;
using LabQ.JwtBearerGrant.Interfaces;
using LabQ.JwtBearerGrant.Models;
using LabQ.JwtBearerGrant.Test.Warmups;
using Microsoft.Extensions.DependencyInjection;

namespace LabQ.JwtBearerGrant.Test.StoreTests;
public abstract class GenericStoreServiceTest<TWarmup> : IClassFixture<TWarmup>
    where TWarmup : class, IWarmupTest
{
    protected readonly IAccessTokenStore _store;
    public TWarmup WarmupData { get; }

    public GenericStoreServiceTest(TWarmup warmup)
    {
        WarmupData = warmup;
        _store = WarmupData.Services.GetRequiredService<IAccessTokenStore>();
        WarmupData.Clear();
    }

    protected JwtBearerToken GenerateRandomToken()
    {
        return new Faker<JwtBearerToken>()
            .StrictMode(true)
            .RuleFor(o => o.Id, f => f.Random.Uuid())
            .RuleFor(o => o.Subject, f => f.Internet.UserName())
            .RuleFor(o => o.AccessToken, f => f.System.ApplePushToken())
            .RuleFor(o => o.Scope, f => f.Random.String(length: 20))
            .RuleFor(o => o.TokenType, f => "Bearer")
            .RuleFor(o => o.CreatedAt, f => DateTime.UtcNow)
            .RuleFor(o => o.ExpiredAt, f => f.Date.Soon());
    }

    [Fact]
    public async Task Should_Save_Token()
    {
        // Arrage
        var token = GenerateRandomToken();

        // Act
        var storeToken = () => _store.Store(token);

        await storeToken.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Should_Not_Save_Expired_Token()
    {
        // Arrage 
        var token = GenerateRandomToken();
        token.ExpiredAt.AddMinutes(-10);

        // Act
        await _store.Store(token);
    }

    [Fact]
    public async Task Should_Return_Correct_Token_By_Id()
    {
        // Arrange
        var token = GenerateRandomToken();

        // Act
        await _store.Store(token);
        var current = await _store.Get(token.Id);

        // Assert
        current.Should().NotBeNull();
        current!.Id.Should().Be(token.Id);
    }

    [Fact]
    public async Task Should_Return_Correct_Token_By_Subject()
    {
        // Arrange
        var token = GenerateRandomToken();

        // Act
        await _store.Store(token);
        var current = await _store.GetLatestFor(token.Subject!);

        // Assert
        current.Should().NotBeNull();
        current!.Subject.Should().Be(token.Subject);
    }

    [Fact]
    public async Task Should_Not_Return_Expired_Token()
    {
        // Arrange
        var token = GenerateRandomToken();
        token.ExpiredAt = DateTime.UtcNow.AddSeconds(3);

        // Act
        await _store.Store(token);
        Thread.Sleep(4000);
        var current = await _store.Get(token.Id);

        // Assert
        current.Should().BeNull();
    }
}
