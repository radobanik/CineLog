namespace CineLog.Mobile.Core.Models.Home;

public sealed class MovieItem
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? PosterPath { get; init; }
    public double? AverageRating { get; init; }

    public string RatingText =>
        AverageRating.HasValue ? AverageRating.Value.ToString("0.0") : "-";
}
