namespace CineLog.TmdbSync.Infrastructure;

public static class RetryHelper
{
    public static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation,
        int maxAttempts = 3,
        CancellationToken ct = default)
    {
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (ex is not OperationCanceledException && attempt < maxAttempts)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)); // 2s, 4s, 8s, ...
                await Task.Delay(delay, ct);
            }
        }

        return await operation();
    }

    public static async Task ExecuteAsync(
        Func<Task> operation,
        int maxAttempts = 3,
        CancellationToken ct = default)
    {
        await ExecuteAsync(async () => { await operation(); return 0; }, maxAttempts, ct);
    }
}
