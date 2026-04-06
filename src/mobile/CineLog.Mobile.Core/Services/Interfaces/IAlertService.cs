namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IAlertService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task ShowToastAsync(string message);
}
