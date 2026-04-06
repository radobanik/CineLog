namespace CineLog.Application.Common;

public interface IBlobStorageService
{
    /// <summary>Upload a file and return its permanent public URL.</summary>
    Task<string> UploadAsync(string key, Stream content, string contentType, CancellationToken ct = default);

    Task DeleteAsync(string key, CancellationToken ct = default);
}
