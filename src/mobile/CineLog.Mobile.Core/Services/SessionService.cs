using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class SessionService : ISessionService
{
    private const string TokenKey = "cinelog_token";
    private const string UserIdKey = "cinelog_userid";
    private const string UsernameKey = "cinelog_username";

    private readonly ISecureStorageService _storage;

    private string? _token;
    private Guid _userId;
    private string? _username;

    public SessionService(ISecureStorageService storage)
    {
        _storage = storage;
    }

    public string? Token => _token;
    public Guid UserId => _userId;
    public string? Username => _username;
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_token);

    public async Task SetSessionAsync(string token, Guid userId, string username)
    {
        _token = token;
        _userId = userId;
        _username = username;

        await _storage.SetAsync(TokenKey, token);
        await _storage.SetAsync(UserIdKey, userId.ToString());
        await _storage.SetAsync(UsernameKey, username);
    }

    public void ClearSession()
    {
        _token = null;
        _userId = Guid.Empty;
        _username = null;

        _storage.Remove(TokenKey);
        _storage.Remove(UserIdKey);
        _storage.Remove(UsernameKey);
    }

    public async Task<bool> TryRestoreSessionAsync()
    {
        var token = await _storage.GetAsync(TokenKey);
        var userIdStr = await _storage.GetAsync(UserIdKey);
        var username = await _storage.GetAsync(UsernameKey);

        if (string.IsNullOrWhiteSpace(token) || !Guid.TryParse(userIdStr, out var userId))
            return false;

        _token = token;
        _userId = userId;
        _username = username;
        return true;
    }
}
