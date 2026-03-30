using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CineLog.Application.Features.Auth;
using CineLog.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace CineLog.IntegrationTests.Endpoints;

public class AuthEndpointTests : IClassFixture<CineLogWebApplicationFactory>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthEndpointTests(CineLogWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static object RegisterPayload(string? email = null, string? username = null) => new
    {
        Username = username ?? $"user{Guid.NewGuid():N}"[..10],
        Email = email ?? $"{Guid.NewGuid()}@test.com",
        Password = "Password123!"
    };

    [Fact]
    public async Task Register_ValidRequest_Returns200WithToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", RegisterPayload());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var auth = JsonSerializer.Deserialize<AuthResponse>(body, JsonOpts)!;
        auth.Token.Should().NotBeNullOrWhiteSpace();
        auth.UserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var email = $"{Guid.NewGuid()}@test.com";

        await _client.PostAsJsonAsync("/api/auth/register", RegisterPayload(email));
        var second = await _client.PostAsJsonAsync("/api/auth/register", RegisterPayload(email));

        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var email = $"{Guid.NewGuid()}@test.com";
        var payload = RegisterPayload(email);
        await _client.PostAsJsonAsync("/api/auth/register", payload);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = "Password123!"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await loginResponse.Content.ReadAsStringAsync();
        var auth = JsonSerializer.Deserialize<AuthResponse>(body, JsonOpts)!;
        auth.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var email = $"{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/auth/register", RegisterPayload(email));

        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = "WrongPassword!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
