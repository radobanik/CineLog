using System.Windows.Input;

namespace CineLog.Mobile.Views.WatchList;

public partial class WatchListNameFormView : ContentView
{
    public static readonly BindableProperty IsOpenProperty =
        BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(WatchListNameFormView));

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(WatchListNameFormView), string.Empty);

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(WatchListNameFormView), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty ConfirmCommandProperty =
        BindableProperty.Create(nameof(ConfirmCommand), typeof(ICommand), typeof(WatchListNameFormView));

    public static readonly BindableProperty CancelCommandProperty =
        BindableProperty.Create(nameof(CancelCommand), typeof(ICommand), typeof(WatchListNameFormView));

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public ICommand? ConfirmCommand
    {
        get => (ICommand?)GetValue(ConfirmCommandProperty);
        set => SetValue(ConfirmCommandProperty, value);
    }

    public ICommand? CancelCommand
    {
        get => (ICommand?)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    public WatchListNameFormView()
    {
        InitializeComponent();
    }
}
