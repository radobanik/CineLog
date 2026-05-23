using System;
using System.Collections.Generic;
using System.Text;

namespace CineLog.Domain.Enums
{
    public enum ActivityType
    {
        MovieWatched = 1,
        MovieWatchLaterAdded = 2,
        MovieFavorited = 3,
        MovieAddedToCustomWatchlist = 4,
        ReviewCreated = 5,
        ReviewLiked = 6,
        UserFollowed = 7
    }
}
