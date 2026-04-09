using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using static System.Net.WebRequestMethods;


namespace CineLog.Mobile.Core.ViewModels.Common
{
    public partial class MoviesCategoryViewModel : BaseViewModel
    {
        private const int PageSize = 24;

        private readonly IHomeService _homeService;

        private int _currentCount = PageSize;
        private MovieCategory _category = MovieCategory.TopRated;

        [ObservableProperty]
        private bool _hasMore = true;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isLoadingMore;

        public ObservableCollection<HomeMovieItem> Movies { get; } = [];

        public MoviesCategoryViewModel(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public void SetCategory(MovieCategory category)
        {
            _category = category;
            Title = _category == MovieCategory.TopRated ? "Top Rated" : "New Releases";
        }

        [RelayCommand]
        public async Task Load()
        {
            await ExecuteAsync(async () =>
            {
                HasError = false;
                ErrorMessage = string.Empty;
                _currentCount = PageSize;
                HasMore = true;

                var movies = await GetMoviesAsync(_currentCount);

                Movies.Clear();
                foreach (var movie in movies)
                    Movies.Add(movie);
            });
        }

        [RelayCommand]
        public async Task LoadMore()
        {
            if (IsBusy || IsLoadingMore || !HasMore)
                return;

            try
            {
                IsLoadingMore = true;

                var previousCount = Movies.Count;
                _currentCount += PageSize;

                var movies = await GetMoviesAsync(_currentCount);

                var existingIds = Movies.Select(x => x.Id).ToHashSet();
                foreach (var movie in movies.Where(x => !existingIds.Contains(x.Id)))
                    Movies.Add(movie);

                if (Movies.Count == previousCount)
                    HasMore = false;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoadingMore = false;
            }
        }

        private Task<IReadOnlyList<HomeMovieItem>> GetMoviesAsync(int count)
        {
            return _category == MovieCategory.TopRated
                ? _homeService.GetTopRatedMoviesAsync(count)
                : _homeService.GetNewReleaseMoviesAsync(count);
        }

        protected override Task OnError(Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
            return Task.CompletedTask;
        }
    }
}
