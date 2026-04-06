using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.People.SetPersonPhoto;

public class SetPersonPhotoHandler(
    IBlobStorageService blobStorage,
    IPersonRepository personRepository,
    IAppDbContext context) : IRequestHandler<SetPersonPhotoCommand, SetPersonPhotoResponse>
{
    public async Task<SetPersonPhotoResponse> Handle(SetPersonPhotoCommand request, CancellationToken cancellationToken)
    {
        var person = await personRepository.GetByIdAsync(request.PersonId, cancellationToken)
            ?? throw new NotFoundException($"Person {request.PersonId} not found.");

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var key = $"people/{request.PersonId}/photo{extension}";

        var url = await blobStorage.UploadAsync(key, request.FileStream, request.ContentType, cancellationToken);

        person.ProfilePath = url;
        await context.SaveChangesAsync(cancellationToken);

        return new SetPersonPhotoResponse(url);
    }
}
