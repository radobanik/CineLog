using CineLog.Mobile.Helpers;

namespace CineLog.Mobile.Controls;

public partial class NavBar : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(NavBar), string.Empty);

    public static readonly BindableProperty ShowMenuButtonProperty =
        BindableProperty.Create(nameof(ShowMenuButton), typeof(bool), typeof(NavBar), false,
            propertyChanged: (b, _, n) => ((NavBar)b).LeftIcon = (bool)n
                ? FontAwesomeIcons.Bars
                : FontAwesomeIcons.ChevronLeft);

    public static readonly BindableProperty LeftIconProperty =
        BindableProperty.Create(nameof(LeftIcon), typeof(string), typeof(NavBar), FontAwesomeIcons.ChevronLeft);

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

    public NavBar()
    {
        InitializeComponent();
    }

    private async void OnLeftTapped(object sender, TappedEventArgs e)
    {
        if (!ShowMenuButton)
            await Shell.Current.GoToAsync("..");
    }
}
