using System.Windows.Input;

namespace CineLog.Mobile.Views.WatchList;

public partial class WatchListRowView : ContentView
{
    public static readonly BindableProperty OpenCommandProperty =
        BindableProperty.Create(nameof(OpenCommand), typeof(ICommand), typeof(WatchListRowView));

    public static readonly BindableProperty ToggleOptionsCommandProperty =
        BindableProperty.Create(nameof(ToggleOptionsCommand), typeof(ICommand), typeof(WatchListRowView));

    public static readonly BindableProperty EditCommandProperty =
        BindableProperty.Create(nameof(EditCommand), typeof(ICommand), typeof(WatchListRowView));

    public static readonly BindableProperty DeleteCommandProperty =
        BindableProperty.Create(nameof(DeleteCommand), typeof(ICommand), typeof(WatchListRowView));

    public ICommand? OpenCommand
    {
        get => (ICommand?)GetValue(OpenCommandProperty);
        set => SetValue(OpenCommandProperty, value);
    }

    public ICommand? ToggleOptionsCommand
    {
        get => (ICommand?)GetValue(ToggleOptionsCommandProperty);
        set => SetValue(ToggleOptionsCommandProperty, value);
    }

    public ICommand? EditCommand
    {
        get => (ICommand?)GetValue(EditCommandProperty);
        set => SetValue(EditCommandProperty, value);
    }

    public ICommand? DeleteCommand
    {
        get => (ICommand?)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public WatchListRowView()
    {
        InitializeComponent();
    }
}
