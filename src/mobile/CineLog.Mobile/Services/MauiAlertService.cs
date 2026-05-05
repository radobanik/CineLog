using CineLog.Mobile.Core.Services.Interfaces;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace CineLog.Mobile.Services;

public sealed class MauiAlertService : IAlertService
{
    public Task ShowAlertAsync(string title, string message, string cancel = "OK") =>
        Application.Current!.Windows[0].Page!.DisplayAlert(title, message, cancel);

    public Task<bool> ShowConfirmAsync(string title, string message, string accept = "Yes", string cancel = "No") =>
        Application.Current!.Windows[0].Page!.DisplayAlert(title, message, accept, cancel);

    public async Task ShowToastAsync(string message)
    {
        var toast = Toast.Make(message, ToastDuration.Short);
        await toast.Show();
    }
}
