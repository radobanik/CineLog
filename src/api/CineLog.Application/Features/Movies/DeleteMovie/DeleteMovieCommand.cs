using MediatR;

namespace CineLog.Application.Features.Movies.DeleteMovie;

public record DeleteMovieCommand(Guid MovieId) : IRequest;
