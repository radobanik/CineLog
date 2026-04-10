namespace CineLog.Mobile.Core.ViewModels.Base;

public interface IDataViewModel
{
    Task LoadAsync();
    Task RefreshAsync();
}
