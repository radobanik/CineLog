using CineLog.Application.Features.Movies.SetMoviePoster;
using MediatR;

namespace CineLog.Application.Features.Movies.SetMovieBackdrop;

public record SetMovieBackdropCommand(Guid MovieId, Stream FileStream, string ContentType, string FileName)
    : IRequest<SetMovieImageResponse>;
