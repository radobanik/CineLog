using CineLog.Domain.Entities;
using CineLog.Domain.Events;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Movies.CreateMovie;

public class CreateMovieHandler : IRequestHandler<CreateMovieCommand, Guid>
{
    private readonly IMovieRepository _movieRepository;
    private readonly IPublisher _publisher;

    public CreateMovieHandler(IMovieRepository movieRepository, IPublisher publisher)
    {
        _movieRepository = movieRepository;
        _publisher = publisher;
    }

    public async Task<Guid> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = Movie.Create(request.TmdbId, request.Title, request.Type);

        movie.UpdateDetails(
            request.Overview,
            request.PosterPath,
            request.BackdropPath,
            request.ReleaseDate,
            request.RuntimeMinutes,
            request.ImdbId,
            request.OriginalTitle,
            request.OriginalLanguage,
            request.Tagline,
            request.Status,
            request.Budget,
            request.Revenue,
            request.NumberOfSeasons,
            request.NumberOfEpisodes);

        movie.MarkAsManuallyEdited();

        await _movieRepository.AddAsync(movie, cancellationToken);
        await _publisher.Publish(new MovieSyncEvent(movie.Id), cancellationToken);

        return movie.Id;
    }
}
