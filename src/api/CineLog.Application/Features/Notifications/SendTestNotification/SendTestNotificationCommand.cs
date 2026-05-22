using MediatR;

namespace CineLog.Application.Features.Notifications.SendTestNotification;

public record SendTestNotificationCommand(string Title, string Message) : IRequest;
