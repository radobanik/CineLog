using System;
using System.Collections.Generic;
using System.Text;

namespace CineLog.Mobile.Core.Models.Home
{
    public sealed class HomeMoviePage
    {
        public IReadOnlyList<HomeMovieItem> Items { get; init; } = [];
        public int Page { get; init; }
        public bool HasMore { get; init; }
    }
}
