namespace CineLog.Mobile.Controls;

public partial class SkeletonView : ContentView
{
    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(SkeletonView), new CornerRadius(6));

    public SkeletonView()
    {
        InitializeComponent();
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler != null)
            StartPulse();
        else
            this.AbortAnimation("SkeletonPulse");
    }

    private void StartPulse()
    {
        var animation = new Animation(v => Opacity = v, 1.0, 0.4, Easing.SinInOut);
        animation.Commit(this, "SkeletonPulse", length: 900, repeat: () => true);
    }
}
