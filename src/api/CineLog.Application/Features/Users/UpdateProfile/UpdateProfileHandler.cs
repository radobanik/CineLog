using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.UpdateProfile;

public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, UserProfileResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateProfileHandler(
        IUserRepository userRepository,
        IAppDbContext context,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<UserProfileResponse> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {_currentUser.UserId} not found.");

        user.Bio = request.Bio;
        user.AvatarUrl = request.AvatarUrl;
        await _context.SaveChangesAsync(cancellationToken);

        var filmsCount = await _context.Reviews
            .CountAsync(r => r.UserId == user.Id, cancellationToken);

        var followersCount = await _context.UserFollows
            .CountAsync(f => f.FollowedId == user.Id, cancellationToken);

        var followingCount = await _context.UserFollows
            .CountAsync(f => f.FollowerId == user.Id, cancellationToken);

        return new UserProfileResponse(
            user.Id,
            user.UserName!,
            user.Bio,
            user.AvatarUrl,
            filmsCount,
            followersCount,
            followingCount);
    }
}
