using System.Xml.Linq;
using CineLog.Mobile.Core.Models.WatchList;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineLog.Mobile.Core.ViewModels.WatchList;

public partial class WatchListRowViewModel(WatchListCollectionItem item) : ObservableObject
{
    private const string HeartIcon = "\uf004";
    private const string EyeIcon = "\uf06e";
    private const string ClockIcon = "\uf017";

    public WatchListCollectionItem Item { get; } = item;

    public Guid Id => Item.Id;

    public WatchListType Type => Item.Type;
    public bool IsDefault => Item.IsDefault;
    public bool IsFavorites => Item.IsFavorites;
    public bool CanEdit => Item.CanEdit;
    public bool CanDelete => Item.CanDelete;

    [ObservableProperty]
    private bool _isFirstCustom;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IconText))]
    private string _name = item.Name;

    public int ItemCount
    {
        get => Item.ItemCount;
        set
        {
            if (Item.ItemCount == value)
                return;

            Item.ItemCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CountText));
        }
    }

    public string CountText => ItemCount == 1 ? "1 movie" : $"{ItemCount} movies";

    public bool UsesIconFont =>
        Type is WatchListType.Favorites
            or WatchListType.Watched
            or WatchListType.WatchLater;

    public string IconText =>
     Type switch
     {
         WatchListType.Favorites => HeartIcon,
         WatchListType.Watched => EyeIcon,
         WatchListType.WatchLater => ClockIcon,
         _ => string.IsNullOrWhiteSpace(Name) ? "?" : Name[..1].ToUpperInvariant()
     };
    [ObservableProperty]
    private bool _isOptionsOpen;

    public void RenameLocally(string name)
    {
        Name = name.Trim();
    }
}
