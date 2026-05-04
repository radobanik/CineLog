using CommunityToolkit.Mvvm.ComponentModel;

namespace CineLog.Mobile.Core.Models.Review;

public sealed partial class ReviewListItem : ObservableObject
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public double? Rating { get; init; }
    public string? ReviewText { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }

    [ObservableProperty] private int _likesCount;
    [ObservableProperty] private bool _isLiked;

    public string Initial => (Username?.Length > 0 ? Username[0].ToString() : "?").ToUpper();
    public string DateText => CreatedAt?.ToString("MMM d, yyyy") ?? string.Empty;
    public string DayText => CreatedAt?.ToString("%d") ?? string.Empty;
    public string MonthText => CreatedAt?.ToString("MMM") ?? string.Empty;
}
