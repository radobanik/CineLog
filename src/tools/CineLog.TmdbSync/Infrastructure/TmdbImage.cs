namespace CineLog.TmdbSync.Infrastructure;

internal static class TmdbImage
{
    private const string BaseUrl = "https://image.tmdb.org/t/p/w500";

    internal static string? Url(string? path)
        => string.IsNullOrEmpty(path) ? null : $"{BaseUrl}{path}";
}
