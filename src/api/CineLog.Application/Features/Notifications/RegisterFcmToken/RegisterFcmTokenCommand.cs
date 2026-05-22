using MediatR;

namespace CineLog.Application.Features.Notifications.RegisterFcmToken;

public record RegisterFcmTokenCommand(string Token) : IRequest;
