using CineLog.Application.Common;
using CineLog.Application.Features.Movies.SetMoviePoster;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Movies.SetMovieBackdrop;

public class SetMovieBackdropHandler(
    IBlobStorageService blobStorage,
    IMovieRepository movieRepository,
    IAppDbContext context) : IRequestHandler<SetMovieBackdropCommand, SetMovieImageResponse>
{
    public async Task<SetMovieImageResponse> Handle(SetMovieBackdropCommand request, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(request.MovieId, cancellationToken)
            ?? throw new NotFoundException($"Movie {request.MovieId} not found.");

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var key = $"movies/{request.MovieId}/backdrop{extension}";

        var url = await blobStorage.UploadAsync(key, request.FileStream, request.ContentType, cancellationToken);

        movie.SetBackdropPath(url);
        await context.SaveChangesAsync(cancellationToken);

        return new SetMovieImageResponse(url);
    }
}
