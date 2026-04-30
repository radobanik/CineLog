using System;
using System.Collections.Generic;
using System.Text;

namespace CineLog.Mobile.Core.Models.WatchList
{
    public class WatchListCollectionItem
    {
        public Guid Id {  get; init; }
        public string Name { get; init; } = string.Empty;
        public int ItemCount { get; set; }
        public bool IsFavorites { get; init; }
        public bool CanDelete => !IsFavorites;
    }
}
