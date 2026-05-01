using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.WatchList;

namespace CineLog.Mobile.Services;

public sealed class WatchListNavigationContext : IWatchListNavigationContext
{
    public WatchListRowViewModel? SelectedRow { get; set; }
}
