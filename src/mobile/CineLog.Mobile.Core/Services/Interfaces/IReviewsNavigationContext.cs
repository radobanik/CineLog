using CineLog.Mobile.Core.Navigation;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IReviewsNavigationContext
{
    ReviewsMode Mode { get; set; }

    Guid EntityId { get; set; }

    Guid? FocusReviewId { get; set; }
}
