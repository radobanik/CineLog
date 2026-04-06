using MediatR;

namespace CineLog.Application.Features.Movies.SetMoviePoster;

public record SetMoviePosterCommand(Guid MovieId, Stream FileStream, string ContentType, string FileName)
    : IRequest<SetMovieImageResponse>;

public record SetMovieImageResponse(string Url);
