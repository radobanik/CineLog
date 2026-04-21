using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Android.Window;
using AndroidX.Core.View;
using AView = Android.Views.View;

namespace CineLog.Mobile;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges =
        ConfigChanges.ScreenSize |
        ConfigChanges.Orientation |
        ConfigChanges.UiMode |
        ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize |
        ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
            SplashScreen.SetOnExitAnimationListener(new SplashExitListener());

        WindowCompat.SetDecorFitsSystemWindows(Window!, true);
        base.OnCreate(savedInstanceState);
        WindowCompat.SetDecorFitsSystemWindows(Window!, false);
        Window!.SetSoftInputMode(SoftInput.AdjustNothing);

        var decorView = Window!.DecorView;
        ViewCompat.SetOnApplyWindowInsetsListener(decorView, new InsetHandler());
        ViewCompat.SetWindowInsetsAnimationCallback(decorView, new AnimationBlocker());
        decorView.Post(() => NeutralizeAppBar(decorView));
    }

    public override bool DispatchTouchEvent(MotionEvent? ev)
    {
        if (ev?.Action == MotionEventActions.Down && CurrentFocus is EditText focused)
        {
            var rect = new Android.Graphics.Rect();
            focused.GetGlobalVisibleRect(rect);
            if (!rect.Contains((int)ev.RawX, (int)ev.RawY))
            {
                focused.ClearFocus();
                (GetSystemService(InputMethodService) as InputMethodManager)
                    ?.HideSoftInputFromWindow(focused.WindowToken, HideSoftInputFlags.None);
            }
        }
        return base.DispatchTouchEvent(ev);
    }

    // Pushes content above the keyboard by adjusting DecorView bottom padding
    private sealed class InsetHandler : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat? OnApplyWindowInsets(AView? v, WindowInsetsCompat? insets)
        {
            if (v is null || insets is null) return insets;
            var bars = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
            v.SetPadding(bars.Left, bars.Top, bars.Right, bars.Bottom);
            return WindowInsetsCompat.Consumed;
        }
    }

    // Blocks MAUI's own IME animation handling to prevent double padding during transition
    private sealed class AnimationBlocker : WindowInsetsAnimationCompat.Callback
    {
        public AnimationBlocker() : base(DispatchModeStop) { }

        public override WindowInsetsCompat OnProgress(
            WindowInsetsCompat insets, IList<WindowInsetsAnimationCompat> _) => insets;
    }

    // Prevents AppBarLayout from applying status-bar height a second time
    private static void NeutralizeAppBar(AView root)
    {
        var appBar = FindBySimpleName(root, "AppBarLayout");
        if (appBar is not null)
            ViewCompat.SetOnApplyWindowInsetsListener(appBar,
                new DelegateInsetListener(_ => WindowInsetsCompat.Consumed));
    }

    private static AView? FindBySimpleName(AView view, string name)
    {
        if (view.Class?.SimpleName == name) return view;
        if (view is ViewGroup g)
            for (var i = 0; i < g.ChildCount; i++)
            {
                var r = FindBySimpleName(g.GetChildAt(i)!, name);
                if (r is not null) return r;
            }
        return null;
    }

    private sealed class DelegateInsetListener(
        Func<WindowInsetsCompat?, WindowInsetsCompat?> handler)
        : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat? OnApplyWindowInsets(AView? v, WindowInsetsCompat? insets)
            => handler(insets);
    }

    private sealed class SplashExitListener : Java.Lang.Object, ISplashScreenOnExitAnimationListener
    {
        public void OnSplashScreenExit(SplashScreenView view) => view.Remove();
    }
}
