using CommunityToolkit.Mvvm.ComponentModel;

namespace CineLog.Mobile.Core.ViewModels.Base;

public abstract partial class BaseViewModel : ObservableObject, IViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    public bool IsNotBusy => !IsBusy;

    public virtual Task OnAppearingAsync() => Task.CompletedTask;
    public virtual Task OnDisappearingAsync() => Task.CompletedTask;
    public virtual Task HandleErrorAsync(Exception ex) => throw ex;

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
