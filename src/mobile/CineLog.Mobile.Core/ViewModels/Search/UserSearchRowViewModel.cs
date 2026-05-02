using CineLog.Mobile.Core.Models.Search;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineLog.Mobile.Core.ViewModels.Search;

public partial class UserSearchRowViewModel(UserSearchItem item) : ObservableObject
{
    public UserSearchItem Item { get; } = item;

    public Guid Id => Item.Id;
    public string Username => Item.Username;
    public string? AvatarUrl => Item.AvatarUrl;
    public int ReviewCount => Item.ReviewCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FollowButtonText))]
    private bool _isFollowing = item.IsFollowing;

    public string FollowButtonText => IsFollowing ? "Unfollow" : "Follow";

    public string Initial =>
        string.IsNullOrWhiteSpace(Username)
            ? "?"
            : Username[..1].ToUpperInvariant();

    public string ReviewsText =>
        ReviewCount == 1 ? "1 review" : $"{ReviewCount} reviews";
}
