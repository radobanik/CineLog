using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Users.UploadAvatar;

public class UploadAvatarHandler : IRequestHandler<UploadAvatarCommand, UploadAvatarResponse>
{
    private readonly IBlobStorageService _blobStorage;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAppDbContext _context;

    public UploadAvatarHandler(
        IBlobStorageService blobStorage,
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        IAppDbContext context)
    {
        _blobStorage    = blobStorage;
        _userRepository = userRepository;
        _currentUser    = currentUser;
        _context        = context;
    }

    public async Task<UploadAvatarResponse> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"User {userId} not found.");

        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var key = $"avatars/{userId}{extension}";

        var url = await _blobStorage.UploadAsync(key, request.FileStream, request.ContentType, cancellationToken);

        user.AvatarUrl = url;
        await _context.SaveChangesAsync(cancellationToken);

        return new UploadAvatarResponse(url);
    }
}
