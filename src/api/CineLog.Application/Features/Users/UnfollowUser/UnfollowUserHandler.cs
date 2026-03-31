using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.UnfollowUser;

public class UnfollowUserHandler : IRequestHandler<UnfollowUserCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UnfollowUserHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var follow = await _context.UserFollows
            .FirstOrDefaultAsync(
                f => f.FollowerId == _currentUser.UserId && f.FollowedId == request.TargetUserId,
                cancellationToken)
            ?? throw new NotFoundException("You are not following this user.");

        _context.UserFollows.Remove(follow);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
