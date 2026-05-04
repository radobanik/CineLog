using System.Windows.Input;

namespace CineLog.Mobile.Controls;

public partial class ReviewListCard : ContentView
{
    public static readonly BindableProperty IsMovieModeProperty =
        BindableProperty.Create(nameof(IsMovieMode), typeof(bool), typeof(ReviewListCard), false);

    public static readonly BindableProperty LikeCommandProperty =
        BindableProperty.Create(nameof(LikeCommand), typeof(ICommand), typeof(ReviewListCard));

    public bool IsMovieMode
    {
        get => (bool)GetValue(IsMovieModeProperty);
        set => SetValue(IsMovieModeProperty, value);
    }

    public ICommand? LikeCommand
    {
        get => (ICommand?)GetValue(LikeCommandProperty);
        set => SetValue(LikeCommandProperty, value);
    }

    public ReviewListCard()
    {
        InitializeComponent();
    }
}
