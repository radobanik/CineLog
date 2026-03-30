namespace CineLog.Infrastructure.ExternalApis;

public interface ITmdbClient
{
    Task<TmdbMovieResult?> SearchAsync(string query, CancellationToken ct = default);
    Task<TmdbMovieResult?> GetByIdAsync(int tmdbId, CancellationToken ct = default);
}
