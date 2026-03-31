using MediatR;

namespace CineLog.Application.Features.Users.UnfollowUser;

public record UnfollowUserCommand(Guid TargetUserId) : IRequest;
