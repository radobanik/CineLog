namespace CineLog.Application.Features.Auth;

public interface IJwtService
{
    string GenerateToken(Guid userId, string username, string email, string role);
}
