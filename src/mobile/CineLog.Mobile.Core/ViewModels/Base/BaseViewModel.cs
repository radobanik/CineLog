using CineLog.Mobile.Core.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineLog.Mobile.Core.ViewModels.Base;

public abstract partial class BaseViewModel(IAlertService alerts) : ObservableObject, IViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    public bool IsNotBusy => !IsBusy;

    protected virtual string ErrorTitle => "Error";

    public virtual Task OnAppearingAsync() => ExecuteAsync(LoadAsync);
    public virtual Task OnDisappearingAsync() => ExecuteAsync(UnloadAsync);

    public virtual Task HandleErrorAsync(Exception ex) =>
        alerts.ShowAlertAsync(ErrorTitle, ex.Message);

    protected virtual Task RefreshAsync() => Task.CompletedTask;
    protected virtual Task LoadAsync() => Task.CompletedTask;
    protected virtual Task UnloadAsync() => Task.CompletedTask;

    protected async Task ExecuteAsync(Func<Task> action)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
