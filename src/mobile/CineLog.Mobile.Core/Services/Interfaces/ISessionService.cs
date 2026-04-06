namespace CineLog.Mobile.Core.Services.Interfaces;

public interface ISessionService
{
    string? Token { get; }
    Guid UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }

    Task SetSessionAsync(string token, Guid userId, string username);
    void ClearSession();
    Task<bool> TryRestoreSessionAsync();
}
