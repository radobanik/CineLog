using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Log;

public partial class LogViewModel(INavigationService navigation, IAlertService alerts) : BaseViewModel(alerts)
{
    protected override Task LoadAsync()
    {
        Title = "Log";
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task AddReview() => navigation.NavigateToAsync(Routes.AddReview);
}
