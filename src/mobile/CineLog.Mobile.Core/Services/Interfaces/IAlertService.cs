namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IAlertService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task<bool> ShowConfirmAsync(string title, string message, string accept = "Yes", string cancel = "No");
    Task ShowToastAsync(string message);
}
