using CineLog.Mobile.Core.Models.Search;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Search;

public partial class UserSearchRowViewModel(UserSearchItem item) : ObservableObject
{
    public Guid Id => item.Id;
    public string Username => item.Username;
    public string? AvatarUrl => item.AvatarUrl;
    public int ReviewCount => item.ReviewCount;

    public bool HasAvatar => !string.IsNullOrWhiteSpace(AvatarUrl);
    public bool HasNoAvatar => !HasAvatar;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FollowButtonText))]
    private bool isFollowing = item.IsFollowing;

    public string FollowButtonText => IsFollowing ? "Unfollow" : "Follow";

    public string Initial =>
        string.IsNullOrWhiteSpace(Username)
            ? "?"
            : Username[..1].ToUpperInvariant();

    public string ReviewsText =>
        ReviewCount == 1 ? "1 review" : $"{ReviewCount} reviews";
}
