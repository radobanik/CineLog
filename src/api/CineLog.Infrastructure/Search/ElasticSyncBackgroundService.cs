using CineLog.Application.Common;
using CineLog.Application.Features.Movies.ReindexMovies;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CineLog.Infrastructure.Search;

public sealed class ElasticSyncBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<ElasticSyncBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(Interval, stoppingToken);

            try
            {
                await SyncIfNeededAsync(stoppingToken);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                logger.LogError(ex, "Elasticsearch sync check failed");
            }
        }
    }

    private async Task SyncIfNeededAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();

        var es = scope.ServiceProvider.GetRequiredService<IElasticSearchService>();
        var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var dbCount = await db.Movies.CountAsync(ct);
        var esCount = await es.CountMoviesAsync(ct);

        if (esCount == dbCount) return;

        logger.LogInformation("ES out of sync (db={Db}, es={Es}), reindexing", dbCount, esCount);

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(new ReindexMoviesCommand(), ct);
    }
}
