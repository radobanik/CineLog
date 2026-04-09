using CineLog.Mobile.Core.ViewModels.Home;

namespace CineLog.Mobile.Pages.MainPages;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _vm;

    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadIfNeededCommand.ExecuteAsync(null);
    }

}
