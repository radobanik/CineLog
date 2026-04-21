using CineLog.Mobile.Helpers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using UIKit;

namespace CineLog.Mobile.Platforms.iOS;

public class CustomShellRenderer : ShellRenderer
{
    protected override IShellTabBarAppearanceTracker CreateTabBarAppearanceTracker()
        => new TabBarBorderTracker(base.CreateTabBarAppearanceTracker());
}

public class TabBarBorderTracker(IShellTabBarAppearanceTracker defaultTracker) : IShellTabBarAppearanceTracker
{
    public void Dispose() => defaultTracker.Dispose();

    public void ResetAppearance(UITabBarController controller)
    {
        defaultTracker.ResetAppearance(controller);
        ApplyBorder(controller);
    }

    public void SetAppearance(UITabBarController controller, ShellAppearance appearance)
    {
        defaultTracker.SetAppearance(controller, appearance);
        ApplyBorder(controller);
    }

    public void UpdateLayout(UITabBarController controller)
        => defaultTracker.UpdateLayout(controller);

    private static void ApplyBorder(UITabBarController controller)
    {
        var appearance = new UITabBarAppearance();
        appearance.ConfigureWithOpaqueBackground();
        appearance.BackgroundColor = AppColors.Surface.ToPlatform();
        appearance.ShadowColor = AppColors.Primary.ToPlatform();

        controller.TabBar.StandardAppearance = appearance;
        controller.TabBar.ScrollEdgeAppearance = appearance;
    }
}
