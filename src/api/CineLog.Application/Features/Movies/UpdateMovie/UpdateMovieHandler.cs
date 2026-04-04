using CineLog.Domain.Events;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Movies.UpdateMovie;

public class UpdateMovieHandler : IRequestHandler<UpdateMovieCommand>
{
    private readonly IMovieRepository _movieRepository;
    private readonly IPublisher _publisher;

    public UpdateMovieHandler(IMovieRepository movieRepository, IPublisher publisher)
    {
        _movieRepository = movieRepository;
        _publisher = publisher;
    }

    public async Task Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await _movieRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Movie {request.Id} not found.");

        movie.UpdateTitle(request.Title);
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

        await _movieRepository.UpdateAsync(movie, cancellationToken);
        await _publisher.Publish(new MovieSyncEvent(movie.Id), cancellationToken);
    }
}
