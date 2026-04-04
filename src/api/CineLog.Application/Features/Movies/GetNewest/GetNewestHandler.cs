using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Movies.GetNewest;

public class GetNewestHandler : IRequestHandler<GetNewestQuery, List<MovieSummaryResponse>>
{
    private readonly IMovieRepository _movieRepository;

    public GetNewestHandler(IMovieRepository movieRepository)
        => _movieRepository = movieRepository;

    public async Task<List<MovieSummaryResponse>> Handle(
        GetNewestQuery request,
        CancellationToken cancellationToken)
    {
        var movies = await _movieRepository.GetNewestAsync(request.Count, cancellationToken);

        return movies
            .Select(m => new MovieSummaryResponse(m.Id, m.Title, m.PosterPath, m.AverageRating, m.Type))
            .ToList();
    }
}
