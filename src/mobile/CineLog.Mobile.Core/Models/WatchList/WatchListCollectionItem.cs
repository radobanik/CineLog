using System;
using System.Collections.Generic;
using System.Text;

namespace CineLog.Mobile.Core.Models.WatchList
{
    public class WatchListCollectionItem
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int ItemCount { get; set; }
        public DateTimeOffset CreatedAt { get; init; }
        public WatchListType Type { get; init; } = WatchListType.Custom;

        public bool IsFavorites => Type == WatchListType.Favorites;
        public bool IsDefault => Type is WatchListType.Favorites or WatchListType.WatchLater or WatchListType.Watched;
        public bool CanEdit => Type == WatchListType.Custom;
        public bool CanDelete => Type == WatchListType.Custom;
    }
}
