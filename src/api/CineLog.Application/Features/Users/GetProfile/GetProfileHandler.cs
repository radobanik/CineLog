using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.GetProfile;

public class GetProfileHandler : IRequestHandler<GetProfileQuery, UserProfileResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAppDbContext _context;

    public GetProfileHandler(IUserRepository userRepository, IAppDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<UserProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} not found.");

        var filmsCount = await _context.Reviews
            .CountAsync(r => r.UserId == request.UserId, cancellationToken);

        var followersCount = await _context.UserFollows
            .CountAsync(f => f.FollowedId == request.UserId, cancellationToken);

        var followingCount = await _context.UserFollows
            .CountAsync(f => f.FollowerId == request.UserId, cancellationToken);

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
