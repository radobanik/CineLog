using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Movies.SetMoviePoster;

public class SetMoviePosterHandler(
    IBlobStorageService blobStorage,
    IMovieRepository movieRepository,
    IAppDbContext context) : IRequestHandler<SetMoviePosterCommand, SetMovieImageResponse>
{
    public async Task<SetMovieImageResponse> Handle(SetMoviePosterCommand request, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetByIdAsync(request.MovieId, cancellationToken)
            ?? throw new NotFoundException($"Movie {request.MovieId} not found.");

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var key = $"movies/{request.MovieId}/poster{extension}";

        var url = await blobStorage.UploadAsync(key, request.FileStream, request.ContentType, cancellationToken);

        movie.SetPosterPath(url);
        await context.SaveChangesAsync(cancellationToken);

        return new SetMovieImageResponse(url);
    }
}
