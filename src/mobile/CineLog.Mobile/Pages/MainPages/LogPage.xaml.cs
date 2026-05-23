using CineLog.Mobile.Core.ViewModels.Log;

namespace CineLog.Mobile.Pages.MainPages;

public partial class LogPage : BasePage
{
    public LogPage(LogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
