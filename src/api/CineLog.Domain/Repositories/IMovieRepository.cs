using CineLog.Domain.Entities;

namespace CineLog.Domain.Repositories;

public interface IMovieRepository
{
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Movie?> GetByTmdbIdAsync(int tmdbId, CancellationToken cancellationToken = default);
    Task AddAsync(Movie movie, CancellationToken cancellationToken = default);
    Task UpdateAsync(Movie movie, CancellationToken cancellationToken = default);
    Task DeleteAsync(Movie movie, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Movie>> GetTopRatedAsync(int count, CancellationToken cancellationToken = default);
}
