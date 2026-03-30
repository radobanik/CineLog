namespace CineLog.Application.Common;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string Username { get; }
    bool IsAdmin { get; }
}
