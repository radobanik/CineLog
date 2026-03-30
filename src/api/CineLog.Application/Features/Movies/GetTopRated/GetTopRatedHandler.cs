using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Movies.GetTopRated;

public class GetTopRatedHandler : IRequestHandler<GetTopRatedQuery, List<MovieSummaryResponse>>
{
    private readonly IMovieRepository _movieRepository;

    public GetTopRatedHandler(IMovieRepository movieRepository)
        => _movieRepository = movieRepository;

    public async Task<List<MovieSummaryResponse>> Handle(
        GetTopRatedQuery request,
        CancellationToken cancellationToken)
    {
        var movies = await _movieRepository.GetTopRatedAsync(request.Count, cancellationToken);

        return movies
            .Select(m => new MovieSummaryResponse(m.Id, m.Title, m.PosterPath, m.AverageRating, m.Type))
            .ToList();
    }
}
