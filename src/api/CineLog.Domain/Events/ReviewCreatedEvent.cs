using MediatR;

namespace CineLog.Domain.Events;

public record ReviewCreatedEvent(Guid ReviewId, Guid UserId, Guid MovieId) : INotification;
