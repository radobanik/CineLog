using Android.Views.InputMethods;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Services;

public class KeyboardService : IKeyboardService
{
    public void HideKeyboard()
    {
        if (Platform.CurrentActivity is not MainActivity activity) return;

        var focused = activity.CurrentFocus;
        var imm = activity.GetSystemService(Android.Content.Context.InputMethodService)
                  as InputMethodManager;
        imm?.HideSoftInputFromWindow(focused?.WindowToken, HideSoftInputFlags.None);
        focused?.ClearFocus();
    }
}
