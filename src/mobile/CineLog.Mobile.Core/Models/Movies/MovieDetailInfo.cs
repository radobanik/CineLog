namespace CineLog.Mobile.Core.Models.Movies;

public sealed class MovieDetailInfo
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string? Director { get; init; }
    public string? PosterPath { get; init; }
    public string? BackdropPath { get; init; }
    public string? Overview { get; init; }
    public double? AverageRating { get; init; }
    public string RatingText => AverageRating.HasValue ? AverageRating.Value.ToString("0.0") : "-";
    public string RatingsCountText { get; init; } = string.Empty;
    public IReadOnlyList<CastMemberItem> Cast { get; init; } = [];
}
