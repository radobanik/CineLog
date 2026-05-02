using System.Windows.Input;

namespace CineLog.Mobile.Views.WatchList;

public partial class WatchListRowOptionsView : ContentView
{
    public static readonly BindableProperty EditCommandProperty =
        BindableProperty.Create(nameof(EditCommand), typeof(ICommand), typeof(WatchListRowOptionsView));

    public static readonly BindableProperty DeleteCommandProperty =
        BindableProperty.Create(nameof(DeleteCommand), typeof(ICommand), typeof(WatchListRowOptionsView));

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

    public WatchListRowOptionsView()
    {
        InitializeComponent();
    }
}
