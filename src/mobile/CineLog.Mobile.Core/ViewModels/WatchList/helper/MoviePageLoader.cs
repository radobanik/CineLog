using System;
using System.Collections.Generic;
using System.Text;
using CineLog.Mobile.Core.Models;

namespace CineLog.Mobile.Core.ViewModels.WatchList.helper
{
    public sealed class MoviePageLoader
    {
        private const int PageSize = 12;

        private readonly List<MovieItem> _source = [];
        private int _currentPage;

        public int LoadedCount { get; private set; }
        public bool CanLoadMore => LoadedCount < _source.Count;

        public void Reset(IEnumerable<MovieItem> movies)
        {
            _source.Clear();
            _source.AddRange(movies);

            _currentPage = 0;
            LoadedCount = 0;
        }

        public IReadOnlyList<MovieItem> LoadNextPage()
        {
            var page = _source
                .Skip(_currentPage * PageSize)
                .Take(PageSize)
                .ToList();

            _currentPage++;
            LoadedCount += page.Count;

            return page;
        }

        public void Remove(Guid movieId)
        {
            var removedLoadedMovie = _source
                .Take(LoadedCount)
                .Any(movie => movie.Id == movieId);

            _source.RemoveAll(movie => movie.Id == movieId);

            if (removedLoadedMovie)
                LoadedCount = Math.Max(0, LoadedCount - 1);
        }

        public void Clear()
        {
            _source.Clear();
            _currentPage = 0;
            LoadedCount = 0;
        }
    }
}
