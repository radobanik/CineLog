using CineLog.Domain.Events;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Movies.DeleteMovie;

public class DeleteMovieHandler : IRequestHandler<DeleteMovieCommand>
{
    private readonly IMovieRepository _movieRepository;
    private readonly IPublisher _publisher;

    public DeleteMovieHandler(IMovieRepository movieRepository, IPublisher publisher)
    {
        _movieRepository = movieRepository;
        _publisher = publisher;
    }

    public async Task Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await _movieRepository.GetByIdAsync(request.MovieId, cancellationToken)
            ?? throw new NotFoundException($"Movie {request.MovieId} not found.");

        await _movieRepository.DeleteAsync(movie, cancellationToken);
        await _publisher.Publish(new MovieDeletedEvent(request.MovieId), cancellationToken);
    }
}
