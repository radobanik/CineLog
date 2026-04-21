using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using CineLog.Mobile.Helpers;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;

namespace CineLog.Mobile.Platforms.Android;

public class CustomShellRenderer(Context context) : ShellRenderer(context)
{
    protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        => new TabBarBorderTracker(base.CreateBottomNavViewAppearanceTracker(shellItem));
}

public class TabBarBorderTracker(IShellBottomNavViewAppearanceTracker defaultTracker) : IShellBottomNavViewAppearanceTracker
{
    public void Dispose() => defaultTracker.Dispose();

    public void ResetAppearance(BottomNavigationView bottomView)
    {
        defaultTracker.ResetAppearance(bottomView);
        ApplyBorder(bottomView);
    }

    public void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
    {
        defaultTracker.SetAppearance(bottomView, appearance);
        ApplyBorder(bottomView);
    }

    private const int TabBarHeightDp = 60;
    private const int TabBarItemTopPaddingDp = 3;

    private static void ApplyBorder(BottomNavigationView bottomView)
    {
        var density = bottomView.Context?.Resources?.DisplayMetrics?.Density ?? 1f;
        var borderPx = (int)(1.5f * density);

        var border = new ColorDrawable(AppColors.Primary.ToPlatform());
        var background = new ColorDrawable(AppColors.BackgroundDeep.ToPlatform());

        var layers = new LayerDrawable(new Drawable[] { border, background });
        layers.SetLayerInset(1, 0, borderPx, 0, 0);

        bottomView.Background = layers;

        if (bottomView.LayoutParameters is ViewGroup.LayoutParams lp)
        {
            lp.Height = (int)(TabBarHeightDp * density);
            bottomView.LayoutParameters = lp;
        }

        var topPx = (int)(TabBarItemTopPaddingDp * density);
        bottomView.SetPadding(0, topPx, 0, 0);
    }
}
