using CineLog.Application.Common;
using CineLog.Domain.Enums;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Movies.SearchMovies;

public class SearchMoviesHandler : IRequestHandler<SearchMoviesQuery, PagedResponse<MovieSummaryResponse>>
{
    private readonly IElasticSearchService elasticSearch;
    private readonly IAppDbContext context;

    public SearchMoviesHandler(IElasticSearchService elasticSearch, IAppDbContext context)
    {
        this.elasticSearch = elasticSearch;
        this.context = context;
    }

    public async Task<PagedResponse<MovieSummaryResponse>> Handle(
        SearchMoviesQuery request,
        CancellationToken cancellationToken)
    {
        var query = request.Query ?? string.Empty;

        var elasticResult = await elasticSearch.SearchMoviesAsync(
            query,
            request.Page,
            request.PageSize,
            request.Genres,
            cancellationToken);

        if (elasticResult.TotalCount > 0 || !ShouldFallbackToDatabase(request))
            return MapElasticResult(elasticResult);

        return await SearchDatabaseAsync(request, cancellationToken);
    }

    private static bool ShouldFallbackToDatabase(SearchMoviesQuery request) =>
        string.IsNullOrWhiteSpace(request.Query) || request.Genres is { Count: > 0 };

    private static PagedResponse<MovieSummaryResponse> MapElasticResult(
        PagedResponse<MovieSearchDocument> result)
    {
        var items = result.Items
            .Select(movie => new MovieSummaryResponse(
                Guid.Parse(movie.Id),
                movie.Title,
                movie.PosterPath,
                movie.AverageRating,
                Enum.Parse<MovieType>(movie.Type)))
            .ToList();

        return PagedResponse<MovieSummaryResponse>.Create(
            items,
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    private async Task<PagedResponse<MovieSummaryResponse>> SearchDatabaseAsync(
      SearchMoviesQuery request,
      CancellationToken ct)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = context.Movies
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var titleQuery = request.Query.Trim().ToLower();

            query = query.Where(movie =>
                movie.Title.ToLower().Contains(titleQuery));
        }

        var genres = request.Genres?
            .Where(genre => !string.IsNullOrWhiteSpace(genre))
            .Select(genre => genre.Trim().ToLower())
            .ToList();

        if (genres is { Count: > 0 })
        {
            query = query.Where(movie =>
                movie.Genres.Any(movieGenre =>
                    genres.Contains(movieGenre.Genre.Name.ToLower())));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(movie => movie.AverageRating)
            .ThenByDescending(movie => movie.ReleaseDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(movie => new MovieSummaryResponse(
                movie.Id,
                movie.Title,
                movie.PosterPath,
                movie.AverageRating,
                movie.Type))
            .ToListAsync(ct);

        return PagedResponse<MovieSummaryResponse>.Create(
            items,
            page,
            pageSize,
            totalCount);
    }
}
