using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Services.Interfaces;
using Firebase.Messaging;

namespace CineLog.Mobile.Platforms.Android.Services;

public class FcmService : IFcmService
{
    private readonly INotificationsClient _notificationsClient;
    private readonly ISessionService _session;

    public FcmService(INotificationsClient notificationsClient, ISessionService session)
    {
        _notificationsClient = notificationsClient;
        _session = session;
    }

    public async System.Threading.Tasks.Task RegisterTokenAsync(System.Threading.CancellationToken ct = default)
    {
        if (!_session.IsAuthenticated) return;

        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token)) return;

        try
        {
            await _notificationsClient.RegisterFcmTokenAsync(
                new RegisterFcmTokenCommand { Token = token },
                ct);
        }
        catch
        {
            // Non-critical - will sync on next launch or when OnNewToken fires
        }
    }

    internal static System.Threading.Tasks.Task<string?> GetTokenAsync()
    {
        var tcs = new System.Threading.Tasks.TaskCompletionSource<string?>();
        FirebaseMessaging.Instance
            .GetToken()
            .AddOnSuccessListener(new OnSuccessAction(r => tcs.TrySetResult(r?.ToString())))
            .AddOnFailureListener(new OnFailureAction(_ => tcs.TrySetResult(null)));
        return tcs.Task;
    }

    private sealed class OnSuccessAction(Action<Java.Lang.Object?> action)
        : Java.Lang.Object, global::Android.Gms.Tasks.IOnSuccessListener
    {
        public void OnSuccess(Java.Lang.Object? result) => action(result);
    }

    private sealed class OnFailureAction(Action<Java.Lang.Exception?> action)
        : Java.Lang.Object, global::Android.Gms.Tasks.IOnFailureListener
    {
        public void OnFailure(Java.Lang.Exception e) => action(e);
    }
}
