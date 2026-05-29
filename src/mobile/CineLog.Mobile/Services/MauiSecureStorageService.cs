using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Services;

public sealed class MauiSecureStorageService : ISecureStorageService
{
    public async Task<string?> GetAsync(string key)
    {
        try
        {
            return await SecureStorage.Default.GetAsync(key);
        }
        catch
        {
            // Android Keystore can throw CryptographicException after reinstall or Keystore corruption.
            // Treat as missing and clear the bad entry so the user can log in fresh.
            SecureStorage.Default.Remove(key);
            return null;
        }
    }

    public Task SetAsync(string key, string value) =>
        SecureStorage.Default.SetAsync(key, value);

    public void Remove(string key) =>
        SecureStorage.Default.Remove(key);
}
