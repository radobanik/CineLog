using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CineLog.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace CineLog.IntegrationTests.Endpoints;

[Collection(nameof(IntegrationTestCollection))]
public class WatchlistEndpointTests
{
    private readonly CineLogWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public WatchlistEndpointTests(CineLogWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task CreateWatchlist_ValidRequest_Returns201WithId()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/watchlists", new { Name = "Watch Later" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        var id = JsonSerializer.Deserialize<Guid>(body);
        id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_AfterCreating_ContainsWatchlist()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        await client.PostAsJsonAsync("/api/watchlists", new { Name = "My List" });
        var response = await client.GetAsync("/api/watchlists");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetArrayLength().Should().BeGreaterThan(0);
        doc.RootElement.EnumerateArray()
            .Should().Contain(el => el.GetProperty("name").GetString() == "My List");
    }

    [Fact]
    public async Task GetById_ExistingWatchlist_Returns200()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var createResponse = await client.PostAsJsonAsync("/api/watchlists", new { Name = "Detail List" });
        var watchlistId = JsonSerializer.Deserialize<Guid>(await createResponse.Content.ReadAsStringAsync());

        var response = await client.GetAsync($"/api/watchlists/{watchlistId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("id").GetGuid().Should().Be(watchlistId);
        doc.RootElement.GetProperty("name").GetString().Should().Be("Detail List");
        doc.RootElement.GetProperty("movies").GetArrayLength().Should().Be(0);
    }

    [Fact]
    public async Task GetById_NonExistent_Returns404()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync($"/api/watchlists/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddMovie_ThenGetDetail_ContainsMovie()
    {
        var movieId = await _factory.SeedMovieAsync("Watchlist Movie");
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var createResponse = await client.PostAsJsonAsync("/api/watchlists", new { Name = "To Watch" });
        var watchlistId = JsonSerializer.Deserialize<Guid>(await createResponse.Content.ReadAsStringAsync());

        var addResponse = await client.PostAsync($"/api/watchlists/{watchlistId}/movies/{movieId}", null);
        var detail = await client.GetAsync($"/api/watchlists/{watchlistId}");

        addResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var body = await detail.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("movies").GetArrayLength().Should().Be(1);
        doc.RootElement.GetProperty("movies")[0].GetProperty("id").GetGuid().Should().Be(movieId);
    }

    [Fact]
    public async Task AddMovie_Duplicate_Returns409()
    {
        var movieId = await _factory.SeedMovieAsync();
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var createResponse = await client.PostAsJsonAsync("/api/watchlists", new { Name = "Dupes List" });
        var watchlistId = JsonSerializer.Deserialize<Guid>(await createResponse.Content.ReadAsStringAsync());

        await client.PostAsync($"/api/watchlists/{watchlistId}/movies/{movieId}", null);
        var second = await client.PostAsync($"/api/watchlists/{watchlistId}/movies/{movieId}", null);

        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task RemoveMovie_AfterAdding_Returns204AndMovieIsGone()
    {
        var movieId = await _factory.SeedMovieAsync();
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var createResponse = await client.PostAsJsonAsync("/api/watchlists", new { Name = "Remove Test" });
        var watchlistId = JsonSerializer.Deserialize<Guid>(await createResponse.Content.ReadAsStringAsync());

        await client.PostAsync($"/api/watchlists/{watchlistId}/movies/{movieId}", null);
        var removeResponse = await client.DeleteAsync($"/api/watchlists/{watchlistId}/movies/{movieId}");
        var detail = await client.GetAsync($"/api/watchlists/{watchlistId}");

        removeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var body = await detail.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("movies").GetArrayLength().Should().Be(0);
    }

    [Fact]
    public async Task DeleteWatchlist_OwnWatchlist_Returns204AndIsGone()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var createResponse = await client.PostAsJsonAsync("/api/watchlists", new { Name = "Delete Me" });
        var watchlistId = JsonSerializer.Deserialize<Guid>(await createResponse.Content.ReadAsStringAsync());

        var deleteResponse = await client.DeleteAsync($"/api/watchlists/{watchlistId}");
        var getResponse = await client.GetAsync($"/api/watchlists/{watchlistId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_OtherUsersWatchlists_NotVisible()
    {
        var (token1, _) = await _factory.RegisterAsync();
        var (token2, _) = await _factory.RegisterAsync();
        var client1 = _factory.CreateAuthenticatedClient(token1);
        var client2 = _factory.CreateAuthenticatedClient(token2);

        await client1.PostAsJsonAsync("/api/watchlists", new { Name = "User1 Private List" });
        var response = await client2.GetAsync("/api/watchlists");

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.EnumerateArray()
            .Should().NotContain(el => el.GetProperty("name").GetString() == "User1 Private List");
    }
}
