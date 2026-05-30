using CineLog.Mobile.Core.Models.Activity;
using CineLog.Mobile.Core.ViewModels.Log;

namespace CineLog.Mobile.Pages.MainPages;

public partial class LogPage : BasePage
{
    public LogPage(LogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void ToggleReviewExpanded(object? sender, TappedEventArgs e)
    {
        if (sender is Element element &&
            element.BindingContext is ActivityFeedItem item)
        {
            item.ToggleReviewExpanded();
        }
    }
}
