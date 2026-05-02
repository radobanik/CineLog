using System.Windows.Input;

namespace CineLog.Mobile.Views.WatchList;

public partial class WatchListMovieRowView : ContentView
{
    public static readonly BindableProperty RemoveCommandProperty =
        BindableProperty.Create(nameof(RemoveCommand), typeof(ICommand), typeof(WatchListMovieRowView));

    public ICommand? RemoveCommand
    {
        get => (ICommand?)GetValue(RemoveCommandProperty);
        set => SetValue(RemoveCommandProperty, value);
    }

    public WatchListMovieRowView()
    {
        InitializeComponent();
    }
}
