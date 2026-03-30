using CineLog.Application.Common;
using CineLog.Domain.Entities;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Movies.SearchMovies;

public class SearchMoviesHandler : IRequestHandler<SearchMoviesQuery, PagedResponse<MovieSummaryResponse>>
{
    private readonly IMovieSearchService _movieSearchService;
    private readonly IMovieRepository _movieRepository;

    public SearchMoviesHandler(IMovieSearchService movieSearchService, IMovieRepository movieRepository)
    {
        _movieSearchService = movieSearchService;
        _movieRepository = movieRepository;
    }

    public async Task<PagedResponse<MovieSummaryResponse>> Handle(
        SearchMoviesQuery request,
        CancellationToken cancellationToken)
    {
        var searchResults = await _movieSearchService.SearchAsync(request.Query, cancellationToken);

        var upsertTasks = searchResults.Select(data => UpsertMovieAsync(data, cancellationToken));
        var movies = await Task.WhenAll(upsertTasks);

        var totalCount = movies.Length;
        var paged = movies
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MovieSummaryResponse(m.Id, m.Title, m.PosterPath, m.AverageRating, m.Type))
            .ToList();

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PagedResponse<MovieSummaryResponse>(paged, request.Page, request.PageSize, totalCount, totalPages);
    }

    private async Task<Movie> UpsertMovieAsync(MovieSearchData data, CancellationToken ct)
    {
        var existing = await _movieRepository.GetByTmdbIdAsync(data.TmdbId, ct);
        if (existing is not null)
        {
            existing.UpdateDetails(data.Overview, data.PosterPath, data.BackdropPath,
                data.ReleaseDate, data.RuntimeMinutes);
            await _movieRepository.UpdateAsync(existing, ct);
            return existing;
        }

        var movie = Movie.Create(data.TmdbId, data.Title, data.Type);
        movie.UpdateDetails(data.Overview, data.PosterPath, data.BackdropPath,
            data.ReleaseDate, data.RuntimeMinutes);
        await _movieRepository.AddAsync(movie, ct);
        return movie;
    }
}
