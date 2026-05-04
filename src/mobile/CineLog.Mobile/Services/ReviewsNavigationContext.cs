using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Services;

public class ReviewsNavigationContext : IReviewsNavigationContext
{
    public ReviewsMode Mode { get; set; }
    public Guid EntityId { get; set; }
}
