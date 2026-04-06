using CineLog.Mobile.Core.ViewModels.Dashboard;

namespace CineLog.Mobile.Pages.Dashboard;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
