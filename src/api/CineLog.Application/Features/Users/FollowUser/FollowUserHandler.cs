using CineLog.Application.Common;
using CineLog.Domain.Entities;
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

    public FollowUserHandler(
        IUserRepository userRepository,
        IAppDbContext context,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _context = context;
        _currentUser = currentUser;
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
        await _context.SaveChangesAsync(cancellationToken);
    }
}
