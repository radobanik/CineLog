using CineLog.Mobile.Core.Services.Interfaces;
using UIKit;

namespace CineLog.Mobile.Services;

public class KeyboardService : IKeyboardService
{
    public void HideKeyboard() =>
        UIApplication.SharedApplication.KeyWindow?.EndEditing(true);
}
