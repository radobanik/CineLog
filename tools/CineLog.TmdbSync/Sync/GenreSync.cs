using CineLog.TmdbSync.Data;
using CineLog.TmdbSync.Entities;
using CineLog.TmdbSync.Infrastructure;
using DM.MovieApi.MovieDb.Genres;
using Microsoft.EntityFrameworkCore;

namespace CineLog.TmdbSync.Sync;

public class GenreSync(
    TmdbSchemaDbContext db,
    IApiGenreRequest genreApi,
    TmdbRateLimiter rateLimiter,
    ILogger<GenreSync> logger)
{
    public async Task SyncAsync(CancellationToken ct)
    {
        logger.LogInformation("Syncing genres");

        await rateLimiter.ThrottleAsync(ct);
        var movieGenres = await RetryHelper.ExecuteAsync(() => genreApi.GetMoviesAsync(), ct: ct);

        await rateLimiter.ThrottleAsync(ct);
        var tvGenres = await RetryHelper.ExecuteAsync(() => genreApi.GetTelevisionAsync(), ct: ct);

        var allGenreIds = new HashSet<int>(movieGenres.Item?.Select(g => g.Id) ?? []);
        allGenreIds.UnionWith(tvGenres.Item?.Select(g => g.Id) ?? []);

        var existing = await db.Genres
            .Where(g => allGenreIds.Contains(g.Id))
            .ToDictionaryAsync(g => g.Id, ct);

        foreach (var genre in movieGenres.Item ?? [])
            UpsertGenre(existing, db, genre.Id, genre.Name, isMovie: true, isTv: false);

        foreach (var genre in tvGenres.Item ?? [])
        {
            if (existing.TryGetValue(genre.Id, out var g))
                g.IsTvGenre = true;
            else
                UpsertGenre(existing, db, genre.Id, genre.Name, isMovie: false, isTv: true);
        }

        await db.SaveChangesAsync(ct);
        logger.LogInformation("Genres synced: {MovieCount} movie, {TvCount} TV",
            movieGenres.Item?.Count ?? 0, tvGenres.Item?.Count ?? 0);
    }

    private static void UpsertGenre(
        Dictionary<int, TmdbGenre> existing,
        TmdbSchemaDbContext db,
        int id, string name,
        bool isMovie, bool isTv)
    {
        if (existing.TryGetValue(id, out var genre))
        {
            genre.Name = name;
            if (isMovie) genre.IsMovieGenre = true;
            if (isTv) genre.IsTvGenre = true;
        }
        else
        {
            var newGenre = new TmdbGenre { Id = id, Name = name, IsMovieGenre = isMovie, IsTvGenre = isTv };
            db.Genres.Add(newGenre);
            existing[id] = newGenre;
        }
    }
}
