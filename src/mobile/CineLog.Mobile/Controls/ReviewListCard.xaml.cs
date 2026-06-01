using System.Windows.Input;
using CineLog.Mobile.Core.Models.Review;

namespace CineLog.Mobile.Controls;

public partial class ReviewListCard : ContentView
{
    public static readonly BindableProperty IsMovieModeProperty =
        BindableProperty.Create(nameof(IsMovieMode), typeof(bool), typeof(ReviewListCard), false);

    public static readonly BindableProperty IsLatestModeProperty =
        BindableProperty.Create(nameof(IsLatestMode), typeof(bool), typeof(ReviewListCard), false);

    public static readonly BindableProperty LikeCommandProperty =
        BindableProperty.Create(nameof(LikeCommand), typeof(ICommand), typeof(ReviewListCard));

    public static readonly BindableProperty CardCommandProperty =
        BindableProperty.Create(nameof(CardCommand), typeof(ICommand), typeof(ReviewListCard));

    public static readonly BindableProperty CardCommandParameterProperty =
        BindableProperty.Create(nameof(CardCommandParameter), typeof(object), typeof(ReviewListCard));

    public bool IsMovieMode
    {
        get => (bool)GetValue(IsMovieModeProperty);
        set => SetValue(IsMovieModeProperty, value);
    }

    public bool IsLatestMode
    {
        get => (bool)GetValue(IsLatestModeProperty);
        set => SetValue(IsLatestModeProperty, value);
    }

    public ICommand? LikeCommand
    {
        get => (ICommand?)GetValue(LikeCommandProperty);
        set => SetValue(LikeCommandProperty, value);
    }

    public ICommand? CardCommand
    {
        get => (ICommand?)GetValue(CardCommandProperty);
        set => SetValue(CardCommandProperty, value);
    }

    public object? CardCommandParameter
    {
        get => GetValue(CardCommandParameterProperty);
        set => SetValue(CardCommandParameterProperty, value);
    }

    public ReviewListCard()
    {
        InitializeComponent();
    }
}
