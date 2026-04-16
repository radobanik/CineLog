namespace CineLog.Mobile.Core.Models.Review;

public sealed class ReviewItem
{
    public Guid Id { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public double? Rating { get; init; }
    public string? ReviewText { get; init; }
    public int LikesCount { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }

    public string RatingText =>
        Rating.HasValue ? $"{Rating.Value:0.0}/5" : "-";
}
