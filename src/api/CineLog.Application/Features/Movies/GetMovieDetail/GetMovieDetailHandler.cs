using CineLog.Domain.Exceptions;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Movies.GetMovieDetail;

public class GetMovieDetailHandler : IRequestHandler<GetMovieDetailQuery, MovieDetailResponse>
{
    private readonly IMovieRepository _movieRepository;

    public GetMovieDetailHandler(IMovieRepository movieRepository)
        => _movieRepository = movieRepository;

    public async Task<MovieDetailResponse> Handle(GetMovieDetailQuery request, CancellationToken cancellationToken)
    {
        var movie = await _movieRepository.GetByIdAsync(request.MovieId, cancellationToken)
            ?? throw new NotFoundException($"Movie {request.MovieId} not found.");

        return new MovieDetailResponse(
            movie.Id,
            movie.TmdbId,
            movie.Title,
            movie.Overview,
            movie.PosterPath,
            movie.AverageRating,
            movie.RatingsCount,
            movie.Genres.ToList(),
            movie.RuntimeMinutes);
    }
}
