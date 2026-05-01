using System.Xml.Linq;
using CineLog.Mobile.Core.Models.WatchList;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineLog.Mobile.Core.ViewModels.WatchList;

public partial class WatchListRowViewModel(WatchListCollectionItem item) : ObservableObject
{
    public WatchListCollectionItem Item { get; } = item;

    public Guid Id => Item.Id;
    public bool IsFavorites => Item.IsFavorites;
    public bool CanEdit => Item.CanEdit;
    public bool CanDelete => Item.CanDelete;

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

    public string IconText =>
        IsFavorites
            ? "\uf004"
            : string.IsNullOrWhiteSpace(Name)
                ? "?"
                : Name[..1].ToUpperInvariant();

    [ObservableProperty]
    private bool _isOptionsOpen;

    public void RenameLocally(string name)
    {
        Name = name.Trim();
    }
}
