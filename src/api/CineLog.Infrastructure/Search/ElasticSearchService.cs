using CineLog.Application.Common;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;

namespace CineLog.Infrastructure.Search;

public class ElasticSearchService : IElasticSearchService
{
    private const string MoviesIndex = "cinelog-movies";
    private const string PeopleIndex = "cinelog-people";

    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticSearchService> _logger;

    public ElasticSearchService(ElasticsearchClient client, ILogger<ElasticSearchService> logger)
    {
        _client = client;
        _logger = logger;
    }

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
        string query, int page, int pageSize, IEnumerable<string>? genres = null, CancellationToken ct = default)
    {
        var genreList = genres?.ToList();

        var response = await _client.SearchAsync<MovieSearchDocument>(s => s
            .Index(MoviesIndex)
            .From((page - 1) * pageSize)
            .Size(pageSize)
            .Query(q => q
                .Bool(b =>
                {
                    b.Should(
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
                    );
                    b.MinimumShouldMatch(1);

                    if (genreList is { Count: > 0 })
                        b.Filter(f => f.Terms(t => t
                            .Field("genres")
                            .Terms(new TermsQueryField(
                                genreList.Select(g => FieldValue.String(g)).ToArray()))));
                })
            ), ct);

        var items = response.Documents.ToList();
        var totalCount = (int)response.Total;

        return PagedResponse<MovieSearchDocument>.Create(items, page, pageSize, totalCount);
    }

    public async Task IndexPersonAsync(PersonSearchDocument doc, CancellationToken ct = default)
        => await _client.IndexAsync(doc, i => i.Index(PeopleIndex).Id(doc.Id), ct);

    public async Task DeletePersonAsync(Guid personId, CancellationToken ct = default)
        => await _client.DeleteAsync<PersonSearchDocument>(personId.ToString(), d => d.Index(PeopleIndex), ct);

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

        var operations = docList
            .Select(doc => (IBulkOperation)new BulkIndexOperation<MovieSearchDocument>(doc) { Id = doc.Id })
            .ToList();

        var response = await _client.BulkAsync(new BulkRequest(MoviesIndex) { Operations = operations }, ct);

        if (response.Errors)
            foreach (var item in response.ItemsWithErrors)
                _logger.LogError("ES bulk movie index error [{Id}]: {Error}", item.Id, item.Error?.Reason);
    }

    public async Task BulkIndexPeopleAsync(IEnumerable<PersonSearchDocument> docs, CancellationToken ct = default)
    {
        var docList = docs.ToList();
        if (docList.Count == 0) return;

        var operations = docList
            .Select(doc => (IBulkOperation)new BulkIndexOperation<PersonSearchDocument>(doc) { Id = doc.Id })
            .ToList();

        var response = await _client.BulkAsync(new BulkRequest(PeopleIndex) { Operations = operations }, ct);

        if (response.Errors)
            foreach (var item in response.ItemsWithErrors)
                _logger.LogError("ES bulk people index error [{Id}]: {Error}", item.Id, item.Error?.Reason);
    }
}
