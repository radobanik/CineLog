using System.Xml.Linq;
using CineLog.Mobile.Core.ViewModels.WatchList.helper;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineLog.Mobile.Core.ViewModels.WatchList;

public partial class WatchListNameFormViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOpen))]
    [NotifyPropertyChangedFor(nameof(IsCreateMode))]
    [NotifyPropertyChangedFor(nameof(IsRenameMode))]
    [NotifyPropertyChangedFor(nameof(Title))]
    private WatchListNameFormMode _mode = WatchListNameFormMode.None;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private WatchListRowViewModel? _targetRow;

    public bool IsOpen => Mode is not WatchListNameFormMode.None;
    public bool IsCreateMode => Mode is WatchListNameFormMode.Create;
    public bool IsRenameMode => Mode is WatchListNameFormMode.Rename;

    public string Title =>
        Mode switch
        {
            WatchListNameFormMode.Create => "New watchlist",
            WatchListNameFormMode.Rename => "Rename watchlist",
            _ => string.Empty
        };

    public void BeginCreate()
    {
        TargetRow = null;
        Name = string.Empty;
        Mode = WatchListNameFormMode.Create;
    }

    public void BeginRename(WatchListRowViewModel row)
    {
        TargetRow = row;
        Name = row.Name;
        Mode = WatchListNameFormMode.Rename;
    }

    public void Close()
    {
        TargetRow = null;
        Name = string.Empty;
        Mode = WatchListNameFormMode.None;
    }
}
