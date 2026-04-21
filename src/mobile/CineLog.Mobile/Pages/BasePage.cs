using CineLog.Mobile.Core.ViewModels.Base;
using CineLog.Mobile.Helpers;

namespace CineLog.Mobile.Pages;

public abstract class BasePage : ContentPage
{
    private IViewModel? _viewModel;

    public BasePage()
    {
        BackgroundColor = AppColors.Background;
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
