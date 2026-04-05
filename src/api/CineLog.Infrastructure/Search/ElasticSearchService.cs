using CineLog.Application.Common;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace CineLog.Infrastructure.Search;

public class ElasticSearchService : IElasticSearchService
{
    private const string MoviesIndex = "cinelog-movies";
    private const string PeopleIndex = "cinelog-people";

    private readonly ElasticsearchClient _client;

    public ElasticSearchService(ElasticsearchClient client) => _client = client;

    public async Task EnsureIndicesExistAsync(CancellationToken ct = default)
    {
        var moviesExist = await _client.Indices.ExistsAsync(MoviesIndex, ct);
        if (!moviesExist.Exists)
            await _client.Indices.CreateAsync(MoviesIndex, ct);

        var peopleExist = await _client.Indices.ExistsAsync(PeopleIndex, ct);
        if (!peopleExist.Exists)
            await _client.Indices.CreateAsync(PeopleIndex, ct);
    }

    public async Task<long> CountMoviesAsync(CancellationToken ct = default)
    {
        var response = await _client.CountAsync<MovieSearchDocument>(c => c.Indices(MoviesIndex), ct);
        return response.Count;
    }

    public async Task IndexMovieAsync(MovieSearchDocument doc, CancellationToken ct = default)
        => await _client.IndexAsync(doc, i => i.Index(MoviesIndex).Id(doc.Id), ct);

    public async Task DeleteMovieAsync(Guid movieId, CancellationToken ct = default)
        => await _client.DeleteAsync<MovieSearchDocument>(movieId.ToString(), d => d.Index(MoviesIndex), ct);

    public async Task<PagedResponse<MovieSearchDocument>> SearchMoviesAsync(
        string query, int page, int pageSize, CancellationToken ct = default)
    {
        var response = await _client.SearchAsync<MovieSearchDocument>(s => s
            .Index(MoviesIndex)
            .From((page - 1) * pageSize)
            .Size(pageSize)
            .Query(q => q
                .Bool(b => b
                    .Should(
                        s => s.MultiMatch(m => m
                            .Query(query)
                            .Fields(new[] { "title^3", "originalTitle^2", "overview", "genres" })
                            .Fuzziness(new Fuzziness("AUTO"))
                        ),
                        s => s.MatchPhrasePrefix(m => m
                            .Field("title")
                            .Query(query)
                            .Boost(3)
                        ),
                        s => s.MatchPhrasePrefix(m => m
                            .Field("originalTitle")
                            .Query(query)
                            .Boost(2)
                        )
                    )
                    .MinimumShouldMatch(1)
                )
            ), ct);

        var items = response.Documents.ToList();
        var totalCount = (int)response.Total;

        return PagedResponse<MovieSearchDocument>.Create(items, page, pageSize, totalCount);
    }

    public async Task IndexPersonAsync(PersonSearchDocument doc, CancellationToken ct = default)
        => await _client.IndexAsync(doc, i => i.Index(PeopleIndex).Id(doc.Id), ct);

    public async Task<PagedResponse<PersonSearchDocument>> SearchPeopleAsync(
        string query, int page, int pageSize, CancellationToken ct = default)
    {
        var response = await _client.SearchAsync<PersonSearchDocument>(s => s
            .Index(PeopleIndex)
            .From((page - 1) * pageSize)
            .Size(pageSize)
            .Query(q => q
                .Bool(b => b
                    .Should(
                        s => s.Match(m => m
                            .Field(f => f.Name)
                            .Query(query)
                            .Fuzziness(new Fuzziness("AUTO"))
                        ),
                        s => s.MatchPhrasePrefix(m => m
                            .Field("name")
                            .Query(query)
                        )
                    )
                    .MinimumShouldMatch(1)
                )
            ), ct);

        var items = response.Documents.ToList();
        var totalCount = (int)response.Total;

        return PagedResponse<PersonSearchDocument>.Create(items, page, pageSize, totalCount);
    }

    public async Task BulkIndexMoviesAsync(IEnumerable<MovieSearchDocument> docs, CancellationToken ct = default)
    {
        var docList = docs.ToList();
        if (docList.Count == 0) return;

        await _client.BulkAsync(b => b
            .Index(MoviesIndex)
            .IndexMany(docList, (op, doc) => op.Id(doc.Id)), ct);
    }

    public async Task BulkIndexPeopleAsync(IEnumerable<PersonSearchDocument> docs, CancellationToken ct = default)
    {
        var docList = docs.ToList();
        if (docList.Count == 0) return;

        await _client.BulkAsync(b => b
            .Index(PeopleIndex)
            .IndexMany(docList, (op, doc) => op.Id(doc.Id)), ct);
    }
}
