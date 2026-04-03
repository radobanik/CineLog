using MediatR;

namespace CineLog.Domain.Events;

public record MovieDeletedEvent(Guid MovieId) : INotification;
