namespace CineLog.Application.Features.Movies;

public interface IMovieSearchService
{
    Task<IReadOnlyCollection<MovieSearchData>> SearchAsync(string query, CancellationToken ct = default);
}
