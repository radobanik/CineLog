using CineLog.Mobile.Core.ViewModels.WatchList;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IWatchListNavigationContext
{
    WatchListRowViewModel? SelectedRow { get; set; }
}
