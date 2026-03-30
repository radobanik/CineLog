using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IAppDbContext _context;

    public MovieRepository(IAppDbContext context) => _context = context;

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<Movie?> GetByTmdbIdAsync(int tmdbId, CancellationToken cancellationToken = default) =>
        await _context.Movies
            .AsNoTracking()
            .Cacheable()
            .FirstOrDefaultAsync(m => m.TmdbId == tmdbId, cancellationToken);

    public async Task AddAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _context.Movies.AddAsync(movie, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Movie>> GetTopRatedAsync(int count, CancellationToken cancellationToken = default)
    {
        var results = await _context.Movies
            .AsNoTracking()
            .Cacheable()
            .Where(m => m.RatingsCount > 0)
            .OrderByDescending(m => m.AverageRating)
            .ThenByDescending(m => m.RatingsCount)
            .Take(count)
            .ToListAsync(cancellationToken);

        return results.AsReadOnly();
    }
}
