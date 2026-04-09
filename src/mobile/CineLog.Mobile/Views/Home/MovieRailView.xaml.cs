using System.Collections;
using System.Windows.Input;

namespace CineLog.Mobile.Views.Home;


public partial class MovieRailView : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(MovieRailView), string.Empty);

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(MovieRailView));

    public static readonly BindableProperty LoadMoreCommandProperty =
        BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(MovieRailView));

    public static readonly BindableProperty IsLoadingMoreProperty =
        BindableProperty.Create(nameof(IsLoadingMore), typeof(bool), typeof(MovieRailView), false);

    public MovieRailView()
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
}
