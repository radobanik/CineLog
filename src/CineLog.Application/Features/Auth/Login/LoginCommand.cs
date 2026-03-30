using MediatR;

namespace CineLog.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
