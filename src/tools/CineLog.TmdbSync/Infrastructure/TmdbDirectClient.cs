using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CineLog.TmdbSync.Infrastructure;

public class TmdbDirectClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly TmdbRateLimiter _rateLimiter;
    private static readonly TimeSpan DefaultBackOff = TimeSpan.FromSeconds(10);
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public TmdbDirectClient(string bearerToken, TmdbRateLimiter rateLimiter)
    {
        _rateLimiter = rateLimiter;
        _http = new HttpClient { BaseAddress = new Uri("https://api.themoviedb.org/3/") };
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<TvCreditsDto?> GetTvCreditsAsync(int seriesId, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"tv/{seriesId}/credits", ct);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            var backOff = ParseRetryAfter(response.Headers) ?? DefaultBackOff;
            _rateLimiter.NotifyRateLimited(backOff);
            await Task.Delay(backOff, ct);

            response = await _http.GetAsync($"tv/{seriesId}/credits", ct);
        }

        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TvCreditsDto>(json, JsonOptions);
    }

    private static TimeSpan? ParseRetryAfter(HttpResponseHeaders headers)
    {
        if (headers.RetryAfter?.Delta is { } delta) return delta;
        if (headers.RetryAfter?.Date is { } date) return date - DateTimeOffset.UtcNow;
        return null;
    }

    public void Dispose() => _http.Dispose();
}

public record TvCreditsDto(
    [property: JsonPropertyName("cast")] List<TvCastMemberDto> Cast,
    [property: JsonPropertyName("crew")] List<TvCrewMemberDto> Crew
);

public record TvCastMemberDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("character")] string? Character,
    [property: JsonPropertyName("order")] int Order,
    [property: JsonPropertyName("credit_id")] string? CreditId
);

public record TvCrewMemberDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("department")] string? Department,
    [property: JsonPropertyName("job")] string? Job,
    [property: JsonPropertyName("credit_id")] string? CreditId
);
