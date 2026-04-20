namespace CineLog.Mobile.Controls;

public partial class SkeletonMoviePosterCard : ContentView
{
    public SkeletonMoviePosterCard()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        while (IsLoaded)
        {
            await this.FadeTo(0.3, 700, Easing.SinInOut);
            await this.FadeTo(1.0, 700, Easing.SinInOut);
        }
    }
}
