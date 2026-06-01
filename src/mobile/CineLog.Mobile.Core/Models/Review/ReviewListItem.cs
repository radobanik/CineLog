using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.Models.Review;

public sealed partial class ReviewListItem : ObservableObject
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? AvatarUrl { get; init; }
    public Guid MovieId { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public string? MoviePosterPath { get; init; }
    public double? Rating { get; init; }
    public string? ReviewText { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }

    [ObservableProperty] private int _likesCount;
    [ObservableProperty] private bool _isLiked;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ReviewMaxLines))]
    [NotifyPropertyChangedFor(nameof(ExpandLabel))]
    private bool _isExpanded;

    public string Initial => (Username?.Length > 0 ? Username[0].ToString() : "?").ToUpper();
    public string DateText => CreatedAt?.ToString("MMM d, yyyy") ?? string.Empty;
    public string DayText => CreatedAt?.ToString("%d") ?? string.Empty;
    public string MonthText => CreatedAt?.ToString("MMM") ?? string.Empty;

    public bool HasReviewText => !string.IsNullOrWhiteSpace(ReviewText);
    public bool HasMoviePoster => !string.IsNullOrWhiteSpace(MoviePosterPath);
    public bool IsLongReview => ReviewText?.Length >= 120;
    public int ReviewMaxLines => IsExpanded ? int.MaxValue : 3;
    public string ExpandLabel => IsExpanded ? "Show less" : "Show more";

    [RelayCommand]
    private void ToggleExpanded() => IsExpanded = !IsExpanded;
}
