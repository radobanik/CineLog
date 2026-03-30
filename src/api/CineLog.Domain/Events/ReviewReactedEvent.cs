using CineLog.Domain.Enums;
using MediatR;

namespace CineLog.Domain.Events;

public record ReviewReactedEvent(Guid ReviewId, Guid ReactedByUserId, ReactionType ReactionType) : INotification;
