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

    public ReviewListCard()
    {
        InitializeComponent();
    }

    private void ToggleExpanded(object? sender, TappedEventArgs e)
    {
        if (BindingContext is ReviewListItem review && review.HasReviewText)
            review.IsExpanded = !review.IsExpanded;
    }
}
