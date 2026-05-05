using CineLog.Mobile.Core.ViewModels.Profile;

namespace CineLog.Mobile.Pages.MainPages;

public partial class ProfilePage : BasePage, IQueryAttributable
{
    private readonly ProfileViewModel _vm;

    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("userId", out var value) && TryReadUserId(value) is { } userId)
            _vm.ShowUser(userId);
        else
            _vm.ShowCurrentUser();
    }

    private static Guid? TryReadUserId(object? value)
    {
        if (value is Guid id) return id;
        if (value is string text && Guid.TryParse(Uri.UnescapeDataString(text), out var parsed)) return parsed;
        return null;
    }
}
