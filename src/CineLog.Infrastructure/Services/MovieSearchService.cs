using CineLog.Application.Features.Movies;
using CineLog.Infrastructure.ExternalApis;

namespace CineLog.Infrastructure.Services;

public class MovieSearchService : IMovieSearchService
{
    private readonly ITmdbClient _tmdbClient;

    public MovieSearchService(ITmdbClient tmdbClient)
        => _tmdbClient = tmdbClient;

    public async Task<IReadOnlyCollection<MovieSearchData>> SearchAsync(string query, CancellationToken ct = default)
    {
        var result = await _tmdbClient.SearchAsync(query, ct);
        if (result is null) return [];

        return new[]
        {
            new MovieSearchData(
                result.TmdbId,
                result.Title,
                result.Overview,
                result.PosterPath,
                result.BackdropPath,
                result.ReleaseDate,
                result.Runtime,
                result.Genres,
                result.Type)
        };
    }
}
