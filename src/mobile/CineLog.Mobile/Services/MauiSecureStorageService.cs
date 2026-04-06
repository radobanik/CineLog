using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Services;

public sealed class MauiSecureStorageService : ISecureStorageService
{
    public Task<string?> GetAsync(string key) =>
        SecureStorage.Default.GetAsync(key);

    public Task SetAsync(string key, string value) =>
        SecureStorage.Default.SetAsync(key, value);

    public void Remove(string key) =>
        SecureStorage.Default.Remove(key);
}
