using System.Windows.Input;

namespace CineLog.Mobile.Controls;

public partial class FollowToggleButtonView : ContentView
{
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FollowToggleButtonView));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FollowToggleButtonView));

    public static readonly BindableProperty IsFollowingProperty =
        BindableProperty.Create(nameof(IsFollowing), typeof(bool), typeof(FollowToggleButtonView));

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FollowToggleButtonView), "Follow");

    public ICommand? Command { get => (ICommand?)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
    public object? CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }
    public bool IsFollowing { get => (bool)GetValue(IsFollowingProperty); set => SetValue(IsFollowingProperty, value); }
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public FollowToggleButtonView() => InitializeComponent();
}
