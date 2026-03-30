namespace CineLog.TmdbSync.Infrastructure;

/// <summary>
/// Sliding-window rate limiter: 38 requests per 10 seconds (safely under TMDb's 40/10s limit).
/// </summary>
public sealed class TmdbRateLimiter : IDisposable
{
    private const int MaxRequests = 38;
    private static readonly TimeSpan WindowSize = TimeSpan.FromSeconds(10);

    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly Queue<DateTimeOffset> _requestTimestamps = new();

    public async Task ThrottleAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var now = DateTimeOffset.UtcNow;
            var windowStart = now - WindowSize;

            // Evict timestamps outside the current window
            while (_requestTimestamps.Count > 0 && _requestTimestamps.Peek() <= windowStart)
                _requestTimestamps.Dequeue();

            if (_requestTimestamps.Count >= MaxRequests)
            {
                // Wait until the oldest request falls outside the window
                var oldest = _requestTimestamps.Peek();
                var waitUntil = oldest + WindowSize + TimeSpan.FromMilliseconds(50);
                var delay = waitUntil - DateTimeOffset.UtcNow;
                if (delay > TimeSpan.Zero)
                {
                    _lock.Release();
                    await Task.Delay(delay, ct);
                    await _lock.WaitAsync(ct);
                }
                now = DateTimeOffset.UtcNow;
                windowStart = now - WindowSize;
                while (_requestTimestamps.Count > 0 && _requestTimestamps.Peek() <= windowStart)
                    _requestTimestamps.Dequeue();
            }

            _requestTimestamps.Enqueue(DateTimeOffset.UtcNow);
        }
        finally
        {
            if (_lock.CurrentCount == 0)
                _lock.Release();
        }
    }

    public void Dispose() => _lock.Dispose();
}
