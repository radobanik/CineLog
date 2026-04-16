namespace CineLog.Mobile.Core.ViewModels.Base;

public interface IViewModel
{
    string Title { get; }
    bool IsBusy { get; }
    Task OnAppearingAsync();
    Task OnDisappearingAsync();
    Task HandleErrorAsync(Exception ex);
}
