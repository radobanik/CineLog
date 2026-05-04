using CineLog.Mobile.Core.ViewModels.Movies;

namespace CineLog.Mobile.Pages.Movies;

public partial class MovieDetailPage : BasePage
{
    public MovieDetailPage(MovieDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.Current.PropertyChanged += OnShellPropertyChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Shell.Current.PropertyChanged -= OnShellPropertyChanged;
    }

    private void OnShellPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Shell.CurrentPage)) return;

        Shell.Current.PropertyChanged -= OnShellPropertyChanged;
        _ = Shell.Current.GoToAsync("..");
    }
}
