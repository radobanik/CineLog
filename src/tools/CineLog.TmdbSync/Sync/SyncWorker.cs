namespace CineLog.TmdbSync.Sync;

public class SyncWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<SyncWorker> logger,
    IConfiguration configuration)
    : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(
        configuration.GetValue("Sync:IntervalMinutes", 1440));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunSyncAsync(stoppingToken);

        using var timer = new PeriodicTimer(_interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
            await RunSyncAsync(stoppingToken);
    }

    private async Task RunSyncAsync(CancellationToken ct)
    {
        logger.LogInformation("Sync cycle started");
        try
        {
            using var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            await sp.GetRequiredService<GenreSync>().SyncAsync(ct);
            await sp.GetRequiredService<MovieFullSync>().SyncAsync(ct);
            await sp.GetRequiredService<TvSeriesFullSync>().SyncAsync(ct);
            await sp.GetRequiredService<PersonSync>().SyncAsync(ct);

            logger.LogInformation("Sync cycle completed");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Sync cycle failed");
        }
    }
}
