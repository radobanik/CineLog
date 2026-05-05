namespace CineLog.Mobile.Views.Search;

public partial class MovieSearchTabView : ContentView
{
    public MovieSearchTabView()
    {
        InitializeComponent();
        SkeletonGrid.ItemsSource = Enumerable.Range(0, 12).ToList();
    }
}
