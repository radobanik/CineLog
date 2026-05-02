namespace CineLog.Mobile.Core.Models.Movies;

public sealed class ReviewPreviewItem
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public double? Rating { get; init; }
    public string? ReviewText { get; init; }
    public int LikesCount { get; init; }
    public string Initial => Username.Length > 0 ? Username[0].ToString().ToUpper() : "?";
}
