using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CineLog.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace CineLog.IntegrationTests.Endpoints;

[Collection(nameof(IntegrationTestCollection))]
public class MoviesEndpointTests
{
    private readonly CineLogWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public MoviesEndpointTests(CineLogWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task GetById_ExistingMovie_Returns200WithData()
    {
        var movieId = await _factory.SeedMovieAsync("Inception");
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync($"/api/movies/{movieId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("title").GetString().Should().Be("Inception");
        doc.RootElement.GetProperty("id").GetGuid().Should().Be(movieId);
    }

    [Fact]
    public async Task GetById_NonExistentMovie_Returns404()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync($"/api/movies/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Unauthenticated_Returns401()
    {
        var movieId = await _factory.SeedMovieAsync();
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/movies/{movieId}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetReviews_ExistingMovie_Returns200WithPagedResult()
    {
        var movieId = await _factory.SeedMovieAsync();
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync($"/api/movies/{movieId}/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("items").GetArrayLength().Should().Be(0);
        doc.RootElement.GetProperty("totalCount").GetInt32().Should().Be(0);
    }

    [Fact]
    public async Task CreateMovie_AsAdmin_Returns201()
    {
        var adminToken = await _factory.LoginAsAdminAsync();
        var client = _factory.CreateAuthenticatedClient(adminToken);

        var response = await client.PostAsJsonAsync("/api/movies", new
        {
            TmdbId = Random.Shared.Next(100_000, int.MaxValue),
            Title  = "Admin Created Movie",
            Type   = 0  // MovieType.Movie
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        var id = JsonSerializer.Deserialize<Guid>(body);
        id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateMovie_AsRegularUser_Returns403()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/movies", new
        {
            TmdbId = 99999,
            Title  = "Should Fail",
            Type   = 0
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
