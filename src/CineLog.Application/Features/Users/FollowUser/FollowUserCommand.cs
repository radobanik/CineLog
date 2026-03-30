using MediatR;

namespace CineLog.Application.Features.Users.FollowUser;

public record FollowUserCommand(Guid TargetUserId) : IRequest;
