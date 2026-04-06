using CineLog.Application.Common;
using CineLog.Application.Features.Users.UploadAvatar;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Users.SetUserAvatar;

public class SetUserAvatarHandler : IRequestHandler<SetUserAvatarCommand, UploadAvatarResponse>
{
    private readonly IBlobStorageService _blobStorage;
    private readonly IUserRepository _userRepository;
    private readonly IAppDbContext _context;

    public SetUserAvatarHandler(
        IBlobStorageService blobStorage,
        IUserRepository userRepository,
        IAppDbContext context)
    {
        _blobStorage = blobStorage;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<UploadAvatarResponse> Handle(SetUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} not found.");

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var key = $"avatars/{request.UserId}{extension}";

        var url = await _blobStorage.UploadAsync(key, request.FileStream, request.ContentType, cancellationToken);

        user.AvatarUrl = url;
        await _context.SaveChangesAsync(cancellationToken);

        return new UploadAvatarResponse(url);
    }
}
