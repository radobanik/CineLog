using System.Collections;
using System.Windows.Input;

namespace CineLog.Mobile.Controls;

public partial class MovieCarousel : ContentView
{
    private const double LoadMoreThreshold = 140;

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(MovieCarousel), string.Empty);

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(MovieCarousel));

    public static readonly BindableProperty LoadMoreCommandProperty =
        BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(MovieCarousel));

    public static readonly BindableProperty IsLoadingMoreProperty =
        BindableProperty.Create(nameof(IsLoadingMore), typeof(bool), typeof(MovieCarousel), false);

    public static readonly BindableProperty SeeAllCommandProperty =
        BindableProperty.Create(nameof(SeeAllCommand), typeof(ICommand), typeof(MovieCarousel));

    public static readonly BindableProperty ShowSeeAllProperty =
        BindableProperty.Create(nameof(ShowSeeAll), typeof(bool), typeof(MovieCarousel), true);

    public MovieCarousel()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public ICommand? LoadMoreCommand
    {
        get => (ICommand?)GetValue(LoadMoreCommandProperty);
        set => SetValue(LoadMoreCommandProperty, value);
    }

    public bool IsLoadingMore
    {
        get => (bool)GetValue(IsLoadingMoreProperty);
        set => SetValue(IsLoadingMoreProperty, value);
    }

    public ICommand? SeeAllCommand
    {
        get => (ICommand?)GetValue(SeeAllCommandProperty);
        set => SetValue(SeeAllCommandProperty, value);
    }

    public bool ShowSeeAll
    {
        get => (bool)GetValue(ShowSeeAllProperty);
        set => SetValue(ShowSeeAllProperty, value);
    }

    private void OnRailScrolled(object? sender, ScrolledEventArgs e)
    {
        if (IsLoadingMore || LoadMoreCommand is null)
            return;

        var distanceToEnd = RailScrollView.ContentSize.Width - (RailScrollView.Width + e.ScrollX);

        if (distanceToEnd > LoadMoreThreshold)
            return;

        if (LoadMoreCommand.CanExecute(null))
            LoadMoreCommand.Execute(null);
    }
}
