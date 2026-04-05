using CineLog.Application.Common;
using MediatR;

namespace CineLog.Application.Features.Movies.SearchMovies;

public record SearchMoviesQuery(string Query, List<string>? Genres = null, int Page = 1, int PageSize = 20)
    : IRequest<PagedResponse<MovieSummaryResponse>>;
