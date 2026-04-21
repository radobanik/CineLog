using MColor = Microsoft.Maui.Graphics.Color;

namespace CineLog.Mobile.Helpers;

public static class AppColors
{
    public static MColor Primary => Get("Primary");
    public static MColor PrimaryDark => Get("PrimaryDark");
    public static MColor PrimaryLight => Get("PrimaryLight");
    public static MColor OnPrimary => Get("OnPrimary");

    public static MColor Background => Get("Background");
    public static MColor BackgroundDeep => Get("BackgroundDeep");

    public static MColor Surface => Get("Surface");
    public static MColor SurfaceVariant => Get("SurfaceVariant");
    public static MColor Overlay => Get("Overlay");

    public static MColor OnBackground => Get("OnBackground");
    public static MColor OnSurface => Get("OnSurface");
    public static MColor TextSecondary => Get("TextSecondary");
    public static MColor TextDisabled => Get("TextDisabled");

    public static MColor Error => Get("Error");
    public static MColor Success => Get("Success");
    public static MColor Warning => Get("Warning");

    public static MColor Border => Get("Border");
    public static MColor Divider => Get("Divider");

    private static MColor Get(string key) =>
        Application.Current?.Resources.TryGetValue(key, out var value) == true && value is MColor color
            ? color
            : Colors.Black;
}
