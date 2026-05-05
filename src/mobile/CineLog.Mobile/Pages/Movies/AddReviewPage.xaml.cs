using CineLog.Mobile.Core.ViewModels.Movies;

namespace CineLog.Mobile.Pages.Movies;

public partial class AddReviewPage : BasePage
{
    private double _panStartRating;

    public AddReviewPage(AddReviewViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnStarsTapped(object sender, TappedEventArgs e)
    {
        if (BindingContext is not AddReviewViewModel vm) return;
        var container = (View)sender;
        var pos = e.GetPosition(container);
        if (pos.HasValue && container.Width > 0)
            vm.Rating = pos.Value.X / container.Width * 5.0;
    }

    private void OnStarsPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (BindingContext is not AddReviewViewModel vm) return;
        var container = (View)sender;
        if (container.Width <= 0) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _panStartRating = vm.Rating;
                break;
            case GestureStatus.Running:
                var startX = _panStartRating / 5.0 * container.Width;
                vm.Rating = (startX + e.TotalX) / container.Width * 5.0;
                break;
        }
    }
}
