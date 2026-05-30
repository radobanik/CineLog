using System.Windows.Input;
using CineLog.Mobile.Helpers;

namespace CineLog.Mobile.Controls;

public partial class NavBar : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(NavBar), string.Empty);

    public static readonly BindableProperty ShowMenuButtonProperty =
        BindableProperty.Create(nameof(ShowMenuButton), typeof(bool), typeof(NavBar), false,
            propertyChanged: (b, _, n) => ((NavBar)b).LeftIcon = (bool)n
                ? string.Empty
                : FontAwesomeIcons.ChevronLeft);

    public static readonly BindableProperty LeftIconProperty =
        BindableProperty.Create(nameof(LeftIcon), typeof(string), typeof(NavBar), FontAwesomeIcons.ChevronLeft);

    public static readonly BindableProperty RightIconProperty =
        BindableProperty.Create(nameof(RightIcon), typeof(string), typeof(NavBar), string.Empty,
            propertyChanged: (b, _, n) => ((NavBar)b).HasRightIcon = !string.IsNullOrEmpty((string)n));

    public static readonly BindableProperty HasRightIconProperty =
        BindableProperty.Create(nameof(HasRightIcon), typeof(bool), typeof(NavBar), false);

    public static readonly BindableProperty RightCommandProperty =
        BindableProperty.Create(nameof(RightCommand), typeof(ICommand), typeof(NavBar), null);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool ShowMenuButton
    {
        get => (bool)GetValue(ShowMenuButtonProperty);
        set => SetValue(ShowMenuButtonProperty, value);
    }

    public string LeftIcon
    {
        get => (string)GetValue(LeftIconProperty);
        set => SetValue(LeftIconProperty, value);
    }

    public string RightIcon
    {
        get => (string)GetValue(RightIconProperty);
        set => SetValue(RightIconProperty, value);
    }

    public bool HasRightIcon
    {
        get => (bool)GetValue(HasRightIconProperty);
        set => SetValue(HasRightIconProperty, value);
    }

    public ICommand? RightCommand
    {
        get => (ICommand?)GetValue(RightCommandProperty);
        set => SetValue(RightCommandProperty, value);
    }

    public NavBar()
    {
        InitializeComponent();
    }

    private async void OnLeftTapped(object sender, TappedEventArgs e)
    {
        if (!ShowMenuButton)
            await Shell.Current.GoToAsync("..");
    }

    private void OnRightTapped(object sender, TappedEventArgs e)
    {
        RightCommand?.Execute(null);
    }
}
