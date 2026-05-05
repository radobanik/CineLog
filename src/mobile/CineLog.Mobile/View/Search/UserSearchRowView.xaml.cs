using System.Windows.Input;

namespace CineLog.Mobile.Views.Search;

public partial class UserSearchRowView : ContentView
{
    public static readonly BindableProperty ToggleFollowCommandProperty =
        BindableProperty.Create(nameof(ToggleFollowCommand), typeof(ICommand), typeof(UserSearchRowView));

    public ICommand? ToggleFollowCommand
    {
        get => (ICommand?)GetValue(ToggleFollowCommandProperty);
        set => SetValue(ToggleFollowCommandProperty, value);
    }

    public UserSearchRowView()
    {
        InitializeComponent();
    }
}
