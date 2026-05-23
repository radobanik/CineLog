using CineLog.Mobile.Core.ViewModels.Movies;
using System.ComponentModel;

namespace CineLog.Mobile.Pages.Movies;

public partial class MovieDetailPage : BasePage
{
    private readonly MovieDetailViewModel _viewModel;

    public MovieDetailPage(MovieDetailViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.Current.PropertyChanged += OnShellPropertyChanged;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Shell.Current.PropertyChanged -= OnShellPropertyChanged;
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnShellPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Shell.CurrentPage)) return;

        Shell.Current.PropertyChanged -= OnShellPropertyChanged;
        _ = Shell.Current.GoToAsync("..");
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MovieDetailViewModel.FocusedReview) || _viewModel.FocusedReview is null)
            return;

        Dispatcher.Dispatch(async () =>
        {
            await Task.Delay(150);
            await DetailScrollView.ScrollToAsync(ReviewsSection, ScrollToPosition.Start, true);
        });
    }
}
