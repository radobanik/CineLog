namespace CineLog.TmdbSync.Infrastructure;

/// <summary>
/// Fixed-interval rate limiter with 429 status code back-off support.
/// Default: 30 ms between requests is circa 33 req/s (TMDb limit is circa 40 req/s).
/// When a 429 is received, call PauseAsync to stall all pending requests.
/// </summary>
public sealed class TmdbRateLimiter : IDisposable
{
    private readonly TimeSpan _minInterval;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private DateTimeOffset _lastRequest = DateTimeOffset.MinValue;
    private DateTimeOffset _pauseUntil = DateTimeOffset.MinValue;

    public TmdbRateLimiter(IConfiguration configuration)
    {
        var ms = configuration.GetValue("Sync:RequestIntervalMs", 30);
        _minInterval = TimeSpan.FromMilliseconds(ms);
    }

    public async Task ThrottleAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var now = DateTimeOffset.UtcNow;

            var pauseDelay = _pauseUntil - now;
            if (pauseDelay > TimeSpan.Zero)
                await Task.Delay(pauseDelay, ct);

            now = DateTimeOffset.UtcNow;
            var intervalDelay = _minInterval - (now - _lastRequest);
            if (intervalDelay > TimeSpan.Zero)
                await Task.Delay(intervalDelay, ct);

            _lastRequest = DateTimeOffset.UtcNow;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void NotifyRateLimited(TimeSpan backOff)
    {
        var until = DateTimeOffset.UtcNow + backOff;
        if (until > _pauseUntil)
            _pauseUntil = until;
    }

    public void Dispose() => _lock.Dispose();
}
