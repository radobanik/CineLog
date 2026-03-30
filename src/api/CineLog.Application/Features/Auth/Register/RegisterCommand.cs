using MediatR;

namespace CineLog.Application.Features.Auth.Register;

public record RegisterCommand(string Username, string Email, string Password) : IRequest<AuthResponse>;
