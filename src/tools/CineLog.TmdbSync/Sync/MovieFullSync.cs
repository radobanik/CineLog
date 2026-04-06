using CineLog.Domain.Enums;
using CineLog.TmdbSync.Data;
using CineLog.TmdbSync.Infrastructure;
using DM.MovieApi.MovieDb.Discover;
using DM.MovieApi.MovieDb.Movies;
using Microsoft.EntityFrameworkCore;
using Genre = CineLog.Domain.Entities.Genre;
using Movie = CineLog.Domain.Entities.Movie;
using MovieCast = CineLog.Domain.Entities.MovieCast;
using MovieCrew = CineLog.Domain.Entities.MovieCrew;
using MovieGenre = CineLog.Domain.Entities.MovieGenre;
using MovieProductionCompany = CineLog.Domain.Entities.MovieProductionCompany;
using Person = CineLog.Domain.Entities.Person;
using ProductionCompany = CineLog.Domain.Entities.ProductionCompany;

namespace CineLog.TmdbSync.Sync;

public class MovieFullSync(
    TmdbSyncDbContext db,
    IApiMovieRequest movieApi,
    IApiDiscoverRequest discoverApi,
    TmdbRateLimiter rateLimiter,
    CheckpointService checkpoints,
    FailureTracker failures,
    ILogger<MovieFullSync> logger,
    IConfiguration configuration)
{
    private const string SyncType = "movies";
    private readonly int _maxPages = configuration.GetValue("Sync:MoviesMaxDiscoverPages", 500);

    public async Task SyncAsync(CancellationToken ct)
    {
        logger.LogInformation("Starting full movie sync");
        var startPage = await checkpoints.GetLastPageAsync(SyncType, ct) + 1;
        var builder = new DiscoverMovieParameterBuilder().SortBy(DiscoverSortBy.Popularity, SortDirection.Desc);

        for (var page = startPage; (_maxPages == -1 || page <= _maxPages) && !ct.IsCancellationRequested; page++)
        {
            await rateLimiter.ThrottleAsync(ct);
            var response = await RetryHelper.ExecuteAsync(
                () => discoverApi.DiscoverMoviesAsync(builder, page),
                ct: ct);

            if (!response.Results.Any()) break;

            foreach (var info in response.Results)
            {
                if (ct.IsCancellationRequested) break;
                await SyncMovieAsync(info, ct);
            }

            await checkpoints.SaveAsync(SyncType, page, response.TotalPages, ct);
            logger.LogInformation("Movies: page {Page}/{Total}", page, response.TotalPages);

            if (page >= response.TotalPages) break;
        }

        await checkpoints.ResetAsync(SyncType, ct);
        logger.LogInformation("Movie sync complete");
    }

    private async Task SyncMovieAsync(MovieInfo info, CancellationToken ct)
    {
        try
        {
            await rateLimiter.ThrottleAsync(ct);
            var detail = await RetryHelper.ExecuteAsync(() => movieApi.FindByIdAsync(info.Id), ct: ct);
            if (detail.Item is null) return;

            var d = detail.Item;
            var existing = await db.Movies.FirstOrDefaultAsync(m => m.IdTmdb == info.Id, ct);

            if (existing is null)
            {
                var movie = Movie.Create(info.Id, info.Title, MovieType.Movie);
                movie.UpdateDetails(
                    info.Overview,
                    TmdbImage.Url(info.PosterPath),
                    TmdbImage.Url(info.BackdropPath),
                    info.ReleaseDate == default ? null : DateOnly.FromDateTime(info.ReleaseDate),
                    d.Runtime,
                    d.ImdbId,
                    d.OriginalTitle,
                    d.OriginalLanguage,
                    d.Tagline,
                    d.Status,
                    (long?)d.Budget,
                    (decimal?)d.Revenue);
                movie.UpdateAverageRating((decimal)info.VoteAverage, info.VoteCount);
                movie.UpdatePopularity(info.Popularity);
                db.Movies.Add(movie);
                await db.SaveChangesAsync(ct);

                await SyncGenresAsync(movie.Id, d.Genres.Select(g => (g.Id, g.Name)), ct);
                await SyncCreditsAsync(movie.Id, info.Id, ct);
                await SyncProductionCompaniesAsync(movie.Id, d.ProductionCompanies, ct);
            }
            else
            {
                existing.UpdateAverageRating((decimal)info.VoteAverage, info.VoteCount);
                existing.UpdatePopularity(info.Popularity);

                if (!existing.IsManuallyEdited)
                {
                    existing.UpdateDetails(
                        info.Overview,
                        TmdbImage.Url(info.PosterPath),
                        TmdbImage.Url(info.BackdropPath),
                        info.ReleaseDate == default ? null : DateOnly.FromDateTime(info.ReleaseDate),
                        d.Runtime,
                        d.ImdbId,
                        d.OriginalTitle,
                        d.OriginalLanguage,
                        d.Tagline,
                        d.Status,
                        (long?)d.Budget,
                        (decimal?)d.Revenue);
                }

                await db.SaveChangesAsync(ct);
                await SyncGenresAsync(existing.Id, d.Genres.Select(g => (g.Id, g.Name)), ct);
                await SyncCreditsAsync(existing.Id, info.Id, ct);
                await SyncProductionCompaniesAsync(existing.Id, d.ProductionCompanies, ct);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Failed to sync movie IdTmdb={IdTmdb}", info.Id);
            db.ChangeTracker.Clear();
            await failures.RecordAsync(SyncType, info.Id, ex, ct);
        }
    }

    private async Task SyncGenresAsync(Guid movieId, IEnumerable<(int IdTmdb, string Name)> genres, CancellationToken ct)
    {
        var oldLinks = await db.MovieGenres.Where(mg => mg.MovieId == movieId).ToListAsync(ct);
        db.MovieGenres.RemoveRange(oldLinks);

        foreach (var g in genres)
        {
            var genreId = await EnsureGenreAsync(g.IdTmdb, g.Name, ct);
            db.MovieGenres.Add(new MovieGenre { MovieId = movieId, GenreId = genreId });
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task SyncCreditsAsync(Guid movieId, int tmdbId, CancellationToken ct)
    {
        await rateLimiter.ThrottleAsync(ct);
        var credits = await RetryHelper.ExecuteAsync(() => movieApi.GetCreditsAsync(tmdbId), ct: ct);
        if (credits.Item is null) return;

        var oldCast = await db.MovieCast.Where(c => c.MovieId == movieId).ToListAsync(ct);
        var oldCrew = await db.MovieCrew.Where(c => c.MovieId == movieId).ToListAsync(ct);
        db.MovieCast.RemoveRange(oldCast);
        db.MovieCrew.RemoveRange(oldCrew);

        foreach (var member in credits.Item.CastMembers)
        {
            var personId = await EnsurePersonAsync(member.PersonId, member.Name, ct);
            db.MovieCast.Add(new MovieCast
            {
                MovieId = movieId,
                PersonId = personId,
                Character = member.Character,
                Order = member.Order,
                CreditId = member.CreditId
            });
        }

        foreach (var member in credits.Item.CrewMembers)
        {
            var personId = await EnsurePersonAsync(member.PersonId, member.Name, ct);
            db.MovieCrew.Add(new MovieCrew
            {
                MovieId = movieId,
                PersonId = personId,
                Department = member.Department,
                Job = member.Job,
                CreditId = member.CreditId
            });
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task SyncProductionCompaniesAsync(
        Guid movieId,
        IEnumerable<DM.MovieApi.MovieDb.Companies.ProductionCompanyInfo> companies,
        CancellationToken ct)
    {
        var oldLinks = await db.MovieProductionCompanies.Where(mp => mp.MovieId == movieId).ToListAsync(ct);
        db.MovieProductionCompanies.RemoveRange(oldLinks);

        foreach (var c in companies)
        {
            var companyId = await EnsureCompanyAsync(c.Id, c.Name, ct);
            db.MovieProductionCompanies.Add(new MovieProductionCompany { MovieId = movieId, CompanyId = companyId });
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task<Guid> EnsurePersonAsync(int idTmdb, string name, CancellationToken ct)
    {
        var tracked = db.ChangeTracker.Entries<Person>().FirstOrDefault(e => e.Entity.IdTmdb == idTmdb);
        if (tracked != null) return tracked.Entity.Id;

        var existing = await db.Persons.FirstOrDefaultAsync(p => p.IdTmdb == idTmdb, ct);
        if (existing != null) return existing.Id;

        var person = new Person { Id = Guid.NewGuid(), IdTmdb = idTmdb, Name = name, SyncedAt = DateTimeOffset.UtcNow };
        db.Persons.Add(person);
        return person.Id;
    }

    private async Task<Guid> EnsureGenreAsync(int idTmdb, string name, CancellationToken ct)
    {
        var tracked = db.ChangeTracker.Entries<Genre>().FirstOrDefault(e => e.Entity.IdTmdb == idTmdb);
        if (tracked != null) return tracked.Entity.Id;

        var existing = await db.Genres.FirstOrDefaultAsync(g => g.IdTmdb == idTmdb, ct);
        if (existing != null) return existing.Id;

        var genre = new Genre { Id = Guid.NewGuid(), IdTmdb = idTmdb, Name = name };
        db.Genres.Add(genre);
        return genre.Id;
    }

    private async Task<Guid> EnsureCompanyAsync(int idTmdb, string name, CancellationToken ct)
    {
        var tracked = db.ChangeTracker.Entries<ProductionCompany>().FirstOrDefault(e => e.Entity.IdTmdb == idTmdb);
        if (tracked != null) return tracked.Entity.Id;

        var existing = await db.ProductionCompanies.FirstOrDefaultAsync(c => c.IdTmdb == idTmdb, ct);
        if (existing != null) return existing.Id;

        var company = new ProductionCompany { Id = Guid.NewGuid(), IdTmdb = idTmdb, Name = name };
        db.ProductionCompanies.Add(company);
        return company.Id;
    }
}
