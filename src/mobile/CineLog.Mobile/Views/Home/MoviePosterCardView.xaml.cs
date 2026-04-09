namespace CineLog.Mobile.Views.Home;


public partial class MoviePosterCardView : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(MoviePosterCardView), string.Empty);

    public static readonly BindableProperty PosterPathProperty =
        BindableProperty.Create(nameof(PosterPath), typeof(string), typeof(MoviePosterCardView));

    public static readonly BindableProperty RatingTextProperty =
        BindableProperty.Create(nameof(RatingText), typeof(string), typeof(MoviePosterCardView), "-");

    public MoviePosterCardView()
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
