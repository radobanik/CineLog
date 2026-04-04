using MediatR;

namespace CineLog.Application.Features.Reviews.ToggleLike;

public record ToggleLikeCommand(Guid ReviewId) : IRequest;
