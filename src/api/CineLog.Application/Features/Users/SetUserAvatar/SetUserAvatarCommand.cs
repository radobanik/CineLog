using CineLog.Application.Features.Users.UploadAvatar;
using MediatR;

namespace CineLog.Application.Features.Users.SetUserAvatar;

public record SetUserAvatarCommand(Guid UserId, Stream FileStream, string ContentType, string FileName)
    : IRequest<UploadAvatarResponse>;
