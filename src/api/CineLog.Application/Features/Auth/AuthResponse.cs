namespace CineLog.Application.Features.Auth;

public record AuthResponse(string Token, Guid UserId, string Username);
