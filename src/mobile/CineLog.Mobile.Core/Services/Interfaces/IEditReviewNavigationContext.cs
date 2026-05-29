namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IEditReviewNavigationContext
{
    Guid ReviewId { get; set; }
    string MovieTitle { get; set; }
    double Rating { get; set; }
    string? ReviewText { get; set; }
}
