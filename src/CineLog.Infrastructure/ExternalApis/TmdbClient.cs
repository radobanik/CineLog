using System.Text.Json;
using System.Text.Json.Serialization;
using CineLog.Domain.Enums;

namespace CineLog.Infrastructure.ExternalApis;

public class TmdbClient : ITmdbClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public TmdbClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<TmdbMovieResult?> SearchAsync(string query, CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient("tmdb");
        var response = await client.GetAsync(
            $"3/search/multi?query={Uri.EscapeDataString(query)}&include_adult=false", ct);

        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<TmdbSearchResponse>(json, JsonOptions);
        var first = result?.Results?.FirstOrDefault();
        return first is null ? null : MapToResult(first);
    }

    public async Task<TmdbMovieResult?> GetByIdAsync(int tmdbId, CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient("tmdb");
        var response = await client.GetAsync($"3/movie/{tmdbId}", ct);

        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync(ct);
        var item = JsonSerializer.Deserialize<TmdbMovieItem>(json, JsonOptions);
        return item is null ? null : MapToResult(item);
    }

    private static TmdbMovieResult MapToResult(TmdbMovieItem item)
    {
        var type = item.MediaType == "tv" ? MovieType.Series : MovieType.Movie;
        var title = item.Title ?? item.Name ?? string.Empty;
        var genres = item.Genres?.Select(g => g.Name).ToList() ?? [];

        DateOnly? releaseDate = null;
        if (DateOnly.TryParse(item.ReleaseDate ?? item.FirstAirDate, out var parsed))
            releaseDate = parsed;

        return new TmdbMovieResult(
            item.Id,
            title,
            item.Overview,
            item.PosterPath,
            item.BackdropPath,
            releaseDate,
            item.Runtime,
            genres.AsReadOnly(),
            type
        );
    }

    // Internal DTOs for TMDb API deserialization
    private record TmdbSearchResponse(
        [property: JsonPropertyName("results")] List<TmdbMovieItem>? Results
    );

    private record TmdbGenre(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name
    );

    private record TmdbMovieItem(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("title")] string? Title,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("overview")] string? Overview,
        [property: JsonPropertyName("poster_path")] string? PosterPath,
        [property: JsonPropertyName("backdrop_path")] string? BackdropPath,
        [property: JsonPropertyName("release_date")] string? ReleaseDate,
        [property: JsonPropertyName("first_air_date")] string? FirstAirDate,
        [property: JsonPropertyName("runtime")] int? Runtime,
        [property: JsonPropertyName("genres")] List<TmdbGenre>? Genres,
        [property: JsonPropertyName("media_type")] string? MediaType
    );
}
