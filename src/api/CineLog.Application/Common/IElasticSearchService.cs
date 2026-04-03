namespace CineLog.Application.Common;

public interface IElasticSearchService
{
    Task EnsureIndicesExistAsync(CancellationToken ct = default);
    Task<long> CountMoviesAsync(CancellationToken ct = default);

    Task IndexMovieAsync(MovieSearchDocument doc, CancellationToken ct = default);
    Task DeleteMovieAsync(Guid movieId, CancellationToken ct = default);
    Task<PagedResponse<MovieSearchDocument>> SearchMoviesAsync(string query, int page, int pageSize, CancellationToken ct = default);

    Task IndexPersonAsync(PersonSearchDocument doc, CancellationToken ct = default);
    Task<PagedResponse<PersonSearchDocument>> SearchPeopleAsync(string query, int page, int pageSize, CancellationToken ct = default);

    Task BulkIndexMoviesAsync(IEnumerable<MovieSearchDocument> docs, CancellationToken ct = default);
    Task BulkIndexPeopleAsync(IEnumerable<PersonSearchDocument> docs, CancellationToken ct = default);
}
