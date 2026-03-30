using CineLog.Domain.Enums;
using MediatR;

namespace CineLog.Application.Features.Reviews.ReactToReview;

public record ReactToReviewCommand(Guid ReviewId, ReactionType ReactionType) : IRequest;
