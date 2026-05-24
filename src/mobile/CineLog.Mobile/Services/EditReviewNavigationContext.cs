using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Services;

public sealed class EditReviewNavigationContext : IEditReviewNavigationContext
{
    public Guid ReviewId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string? ReviewText { get; set; }
}
