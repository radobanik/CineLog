using MediatR;

namespace CineLog.Application.Features.Users.UploadAvatar;

public record UploadAvatarCommand(Stream FileStream, string ContentType, string FileName)
    : IRequest<UploadAvatarResponse>;

public record UploadAvatarResponse(string AvatarUrl);
