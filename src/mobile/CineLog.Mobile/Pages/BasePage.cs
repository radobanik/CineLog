using CineLog.Mobile.Controls;
using CineLog.Mobile.Core.ViewModels.Base;
using CineLog.Mobile.Helpers;

namespace CineLog.Mobile.Pages;

public abstract class BasePage : ContentPage
{
    private IViewModel? _viewModel;
    private bool _navBarInjected;

    public static readonly BindableProperty ShowMenuButtonProperty =
        BindableProperty.Create(nameof(ShowMenuButton), typeof(bool), typeof(BasePage), false);

    public static readonly BindableProperty ShowNavBarProperty =
        BindableProperty.Create(nameof(ShowNavBar), typeof(bool), typeof(BasePage), true);

    public bool ShowMenuButton
    {
        get => (bool)GetValue(ShowMenuButtonProperty);
        set => SetValue(ShowMenuButtonProperty, value);
    }

    public bool ShowNavBar
    {
        get => (bool)GetValue(ShowNavBarProperty);
        set => SetValue(ShowNavBarProperty, value);
    }

    public BasePage()
    {
        BackgroundColor = AppColors.Background;
        Shell.SetNavBarIsVisible(this, false);
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == ContentProperty.PropertyName && !_navBarInjected && ShowNavBar && Content is View pageContent)
        {
            _navBarInjected = true;

            var navBar = new NavBar();
            navBar.SetBinding(NavBar.TitleProperty, new Binding(nameof(Title), source: this));
            navBar.SetBinding(NavBar.ShowMenuButtonProperty, new Binding(nameof(ShowMenuButton), source: this));

            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star)
                }
            };

            Grid.SetRow(navBar, 0);
            Grid.SetRow(pageContent, 1);
            grid.Add(navBar);
            grid.Add(pageContent);

            Content = grid;
        }
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        _viewModel = BindingContext as IViewModel;

        if (_viewModel is not null)
            SetBinding(TitleProperty, new Binding(nameof(IViewModel.Title)));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel is not null)
            _ = _viewModel.OnAppearingAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_viewModel is not null)
            _ = _viewModel.OnDisappearingAsync();
    }
}
