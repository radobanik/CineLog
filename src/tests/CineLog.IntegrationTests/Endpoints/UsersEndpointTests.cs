using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CineLog.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace CineLog.IntegrationTests.Endpoints;

[Collection(nameof(IntegrationTestCollection))]
public class UsersEndpointTests
{
    private readonly CineLogWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public UsersEndpointTests(CineLogWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task GetMe_AuthenticatedUser_Returns200WithProfile()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/users/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("id").GetGuid().Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetMe_Unauthenticated_Returns401()
    {
        var response = await _factory.CreateClient().GetAsync("/api/users/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMe_ValidRequest_Returns200WithUpdatedProfile()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.PutAsJsonAsync("/api/users/me", new
        {
            Bio = "Movie enthusiast",
            AvatarUrl = "https://example.com/avatar.png"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("bio").GetString().Should().Be("Movie enthusiast");
        doc.RootElement.GetProperty("avatarUrl").GetString().Should().Be("https://example.com/avatar.png");
    }

    [Fact]
    public async Task GetById_ExistingUser_Returns200()
    {
        var (token, userId) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync($"/api/users/{userId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("id").GetGuid().Should().Be(userId);
    }

    [Fact]
    public async Task GetById_NonExistentUser_Returns404()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync($"/api/users/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Follow_AnotherUser_Returns204()
    {
        var (token, _) = await _factory.RegisterAsync();
        var (_, targetId) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.PostAsync($"/api/users/{targetId}/follow", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Follow_SameUserTwice_Returns409()
    {
        var (token, _) = await _factory.RegisterAsync();
        var (_, targetId) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        await client.PostAsync($"/api/users/{targetId}/follow", null);
        var second = await client.PostAsync($"/api/users/{targetId}/follow", null);

        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Unfollow_AfterFollowing_Returns204()
    {
        var (token, _) = await _factory.RegisterAsync();
        var (_, targetId) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        await client.PostAsync($"/api/users/{targetId}/follow", null);
        var response = await client.DeleteAsync($"/api/users/{targetId}/follow");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetFollowers_Returns200WithPagedResult()
    {
        var (token, userId) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync($"/api/users/{userId}/followers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("items").ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task GetFavorites_Returns200WithEmptyList()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/users/me/favorites");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetArrayLength().Should().Be(0);
    }

    [Fact]
    public async Task AddToFavorites_ThenGetFavorites_ContainsMovie()
    {
        var movieId = await _factory.SeedMovieAsync("Favorited Film");
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        await client.PostAsync($"/api/movies/{movieId}/favorites", null);
        var response = await client.GetAsync("/api/users/me/favorites");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetArrayLength().Should().Be(1);
        doc.RootElement[0].GetProperty("id").GetGuid().Should().Be(movieId);
    }

    [Fact]
    public async Task RemoveFromFavorites_AfterAdding_Returns204AndMovieIsGone()
    {
        var movieId = await _factory.SeedMovieAsync();
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        await client.PostAsync($"/api/movies/{movieId}/favorites", null);
        var deleteResponse = await client.DeleteAsync($"/api/movies/{movieId}/favorites");
        var favorites = await client.GetAsync("/api/users/me/favorites");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var body = await favorites.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetArrayLength().Should().Be(0);
    }
}
