using CineLog.Mobile.Core.Models.WatchList;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineLog.Mobile.Core.ViewModels.WatchList;

public partial class WatchListRowViewModel(WatchListCollectionItem item) : ObservableObject
{
    public WatchListCollectionItem Item { get; } = item;

    public Guid Id => Item.Id;
    public string Name => Item.Name;
    public int ItemCount => Item.ItemCount;
    public bool IsFavorites => Item.IsFavorites;
    public bool CanEdit => !Item.IsFavorites;
    public bool CanDelete => !Item.IsFavorites;

    [ObservableProperty]
    private bool _isOptionsOpen;

    public string CountText => ItemCount == 1 ? "1 movie" : $"{ItemCount} movies";

    public string IconText =>
        IsFavorites
            ? "\uf004"
            : string.IsNullOrWhiteSpace(Name)
                ? "?"
                : Name[..1].ToUpperInvariant();
    public string? IconFontFamily => IsFavorites ? "FASolid" : null;
}
