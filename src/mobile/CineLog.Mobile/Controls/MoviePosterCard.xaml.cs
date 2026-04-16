namespace CineLog.Mobile.Controls;

public partial class MoviePosterCard : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(MoviePosterCard), string.Empty);

    public static readonly BindableProperty PosterPathProperty =
        BindableProperty.Create(nameof(PosterPath), typeof(string), typeof(MoviePosterCard));

    public static readonly BindableProperty RatingTextProperty =
        BindableProperty.Create(nameof(RatingText), typeof(string), typeof(MoviePosterCard), "-");

    public MoviePosterCard()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? PosterPath
    {
        get => (string?)GetValue(PosterPathProperty);
        set => SetValue(PosterPathProperty, value);
    }

    public string RatingText
    {
        get => (string)GetValue(RatingTextProperty);
        set => SetValue(RatingTextProperty, value);
    }
}
