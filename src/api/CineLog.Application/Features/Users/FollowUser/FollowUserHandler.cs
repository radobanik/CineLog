using CineLog.Application.Common;
using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.FollowUser;

public class FollowUserHandler : IRequestHandler<FollowUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public FollowUserHandler(
        IUserRepository userRepository,
        IAppDbContext context,
        ICurrentUserService currentUser,
        INotificationService notificationService)
    {
        _userRepository = userRepository;
        _context = context;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        if (request.TargetUserId == _currentUser.UserId)
            throw new DomainException("You cannot follow yourself.");

        var target = await _userRepository.GetByIdAsync(request.TargetUserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.TargetUserId} not found.");

        var alreadyFollowing = await _context.UserFollows
            .AnyAsync(f => f.FollowerId == _currentUser.UserId && f.FollowedId == request.TargetUserId,
                cancellationToken);

        if (alreadyFollowing)
            throw new ConflictException("You are already following this user.");

        var follow = UserFollow.Create(_currentUser.UserId, request.TargetUserId);
        await _context.UserFollows.AddAsync(follow, cancellationToken);

        await _context.ActivityLogs.AddAsync(
            ActivityLog.Create(
                _currentUser.UserId,
                ActivityType.UserFollowed,
                targetUserId: request.TargetUserId),
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var followerName = await _context.Users
            .Where(u => u.Id == _currentUser.UserId)
            .Select(u => u.UserName)
            .FirstOrDefaultAsync(cancellationToken);

        await _notificationService.SendAsync(
            request.TargetUserId,
            "New Follower",
            $"{followerName} started following you.",
            cancellationToken);
    }
}
