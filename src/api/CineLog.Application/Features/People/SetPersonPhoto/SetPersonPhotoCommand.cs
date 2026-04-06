using MediatR;

namespace CineLog.Application.Features.People.SetPersonPhoto;

public record SetPersonPhotoCommand(Guid PersonId, Stream FileStream, string ContentType, string FileName)
    : IRequest<SetPersonPhotoResponse>;

public record SetPersonPhotoResponse(string Url);
