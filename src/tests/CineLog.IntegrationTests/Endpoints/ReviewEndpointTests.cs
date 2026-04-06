using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CineLog.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace CineLog.IntegrationTests.Endpoints;

[Collection(nameof(IntegrationTestCollection))]
public class ReviewEndpointTests
{
    private readonly CineLogWebApplicationFactory _factory;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ReviewEndpointTests(CineLogWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task CreateReview_ValidRequest_Returns200WithReview()
    {
        var movieId = await _factory.SeedMovieAsync();
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/reviews", new
        {
            MovieId          = movieId,
            Rating           = 4.0m,
            ReviewText       = "Great film",
            ContainsSpoilers = false
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("rating").GetDecimal().Should().Be(4.0m);
        doc.RootElement.GetProperty("reviewText").GetString().Should().Be("Great film");
        doc.RootElement.GetProperty("id").GetGuid().Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateReview_DuplicateForSameMovie_Returns409()
    {
        var movieId = await _factory.SeedMovieAsync();
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        await client.PostAsJsonAsync("/api/reviews", new
        {
            MovieId = movieId, Rating = 3.0m, ContainsSpoilers = false
        });
        var second = await client.PostAsJsonAsync("/api/reviews", new
        {
            MovieId = movieId, Rating = 4.0m, ContainsSpoilers = false
        });

        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetReview_ExistingReview_Returns200()
    {
        var movieId = await _factory.SeedMovieAsync();
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var created = await client.PostAsJsonAsync("/api/reviews", new
        {
            MovieId = movieId, Rating = 3.5m, ContainsSpoilers = false
        });
        var createdBody = await created.Content.ReadAsStringAsync();
        var reviewId = JsonDocument.Parse(createdBody).RootElement.GetProperty("id").GetGuid();

        var response = await client.GetAsync($"/api/reviews/{reviewId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("id").GetGuid().Should().Be(reviewId);
        doc.RootElement.GetProperty("rating").GetDecimal().Should().Be(3.5m);
    }

    [Fact]
    public async Task GetReview_NonExistent_Returns404()
    {
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync($"/api/reviews/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateReview_OwnReview_Returns200WithUpdatedData()
    {
        var movieId = await _factory.SeedMovieAsync();
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var created = await client.PostAsJsonAsync("/api/reviews", new
        {
            MovieId = movieId, Rating = 2.0m, ReviewText = "Initially bad", ContainsSpoilers = false
        });
        var reviewId = JsonDocument.Parse(await created.Content.ReadAsStringAsync())
            .RootElement.GetProperty("id").GetGuid();

        var response = await client.PutAsJsonAsync($"/api/reviews/{reviewId}", new
        {
            Rating = 5.0m, ReviewText = "Actually amazing on rewatch", ContainsSpoilers = true
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        doc.RootElement.GetProperty("rating").GetDecimal().Should().Be(5.0m);
        doc.RootElement.GetProperty("reviewText").GetString().Should().Be("Actually amazing on rewatch");
        doc.RootElement.GetProperty("containsSpoilers").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task DeleteReview_OwnReview_Returns204AndReviewIsGone()
    {
        var movieId = await _factory.SeedMovieAsync();
        var (token, _) = await _factory.RegisterAsync();
        var client = _factory.CreateAuthenticatedClient(token);

        var created = await client.PostAsJsonAsync("/api/reviews", new
        {
            MovieId = movieId, Rating = 3.0m, ContainsSpoilers = false
        });
        var reviewId = JsonDocument.Parse(await created.Content.ReadAsStringAsync())
            .RootElement.GetProperty("id").GetGuid();

        var deleteResponse = await client.DeleteAsync($"/api/reviews/{reviewId}");
        var getResponse = await client.GetAsync($"/api/reviews/{reviewId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ToggleLike_ExistingReview_Returns204()
    {
        var movieId = await _factory.SeedMovieAsync();
        var (authorToken, _) = await _factory.RegisterAsync();
        var (likerToken, _) = await _factory.RegisterAsync();
        var authorClient = _factory.CreateAuthenticatedClient(authorToken);
        var likerClient = _factory.CreateAuthenticatedClient(likerToken);

        var created = await authorClient.PostAsJsonAsync("/api/reviews", new
        {
            MovieId = movieId, Rating = 4.0m, ContainsSpoilers = false
        });
        var reviewId = JsonDocument.Parse(await created.Content.ReadAsStringAsync())
            .RootElement.GetProperty("id").GetGuid();

        var response = await likerClient.PostAsync($"/api/reviews/{reviewId}/toggle-like", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ToggleLike_TwiceOnSameReview_TogglesOffAndOn()
    {
        var movieId = await _factory.SeedMovieAsync();
        var (authorToken, _) = await _factory.RegisterAsync();
        var (likerToken, _) = await _factory.RegisterAsync();
        var authorClient = _factory.CreateAuthenticatedClient(authorToken);
        var likerClient = _factory.CreateAuthenticatedClient(likerToken);

        var created = await authorClient.PostAsJsonAsync("/api/reviews", new
        {
            MovieId = movieId, Rating = 4.5m, ContainsSpoilers = false
        });
        var reviewId = JsonDocument.Parse(await created.Content.ReadAsStringAsync())
            .RootElement.GetProperty("id").GetGuid();

        var first  = await likerClient.PostAsync($"/api/reviews/{reviewId}/toggle-like", null);
        var second = await likerClient.PostAsync($"/api/reviews/{reviewId}/toggle-like", null);

        first.StatusCode.Should().Be(HttpStatusCode.NoContent);
        second.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
