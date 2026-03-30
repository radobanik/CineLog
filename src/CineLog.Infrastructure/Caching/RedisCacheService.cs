using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace CineLog.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var data = await _cache.GetAsync(key, ct);
        if (data is null) return default;
        return JsonSerializer.Deserialize<T>(data, JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        var data = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
        await _cache.SetAsync(key, data, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        }, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default) =>
        await _cache.RemoveAsync(key, ct);
}
