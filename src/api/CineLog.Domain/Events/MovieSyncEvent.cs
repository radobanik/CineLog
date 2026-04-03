using MediatR;

namespace CineLog.Domain.Events;

public record MovieSyncEvent(Guid MovieId) : INotification;
