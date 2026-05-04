using CommunityToolkit.Mvvm.ComponentModel;

namespace CineLog.Mobile.Core.Models.Movies;

public sealed partial class ReviewPreviewItem : ObservableObject
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public double? Rating { get; init; }
    public string? ReviewText { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }

    [ObservableProperty] private int _likesCount;
    [ObservableProperty] private bool _isLiked;

    public string Initial => Username.Length > 0 ? Username[0].ToString().ToUpper() : "?";

    public string DateText => CreatedAt.HasValue
        ? CreatedAt.Value.ToString("MMM d, yyyy")
        : string.Empty;
}
