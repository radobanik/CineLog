namespace CineLog.Mobile.Core.ViewModels.Base;

public interface IViewModel
{
    bool IsBusy { get; }
    Task OnAppearingAsync();
    Task OnDisappearingAsync();
    Task HandleErrorAsync(Exception ex);
}
