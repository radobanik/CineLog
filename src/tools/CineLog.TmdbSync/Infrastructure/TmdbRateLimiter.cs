namespace CineLog.TmdbSync.Infrastructure;

/// <summary>
/// Simple fixed-interval rate limiter: enforces a minimum delay between every request
/// so we never burst anywhere near TMDb's 40-requests-per-10-second limit.
/// Default: 400 ms between requests ≈ 25 req/10 s.
/// </summary>
public sealed class TmdbRateLimiter : IDisposable
{
    private readonly TimeSpan _minInterval;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private DateTimeOffset _lastRequest = DateTimeOffset.MinValue;

    public TmdbRateLimiter(IConfiguration configuration)
    {
        var ms = configuration.GetValue("Sync:RequestIntervalMs", 400);
        _minInterval = TimeSpan.FromMilliseconds(ms);
    }

    public async Task ThrottleAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var now = DateTimeOffset.UtcNow;
            var elapsed = now - _lastRequest;
            if (elapsed < _minInterval)
                await Task.Delay(_minInterval - elapsed, ct);

            _lastRequest = DateTimeOffset.UtcNow;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose() => _lock.Dispose();
}
