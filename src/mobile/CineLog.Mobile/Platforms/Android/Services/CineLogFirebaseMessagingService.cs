using Android.App;
using AndroidX.Core.App;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Services.Interfaces;
using Firebase.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace CineLog.Mobile.Platforms.Android.Services;

[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class CineLogFirebaseMessagingService : FirebaseMessagingService
{
    private const string ChannelId = "cinelog_default";

    public override void OnNewToken(string token)
    {
        var services = IPlatformApplication.Current?.Services;
        var session = services?.GetService<ISessionService>();
        if (session?.IsAuthenticated != true) return;

        var client = services?.GetService<INotificationsClient>();
        if (client is null) return;

        _ = client.RegisterFcmTokenAsync(new RegisterFcmTokenCommand { Token = token });
    }

    public override void OnMessageReceived(RemoteMessage message)
    {
        var notification = message.GetNotification();
        if (notification is null) return;

        ShowLocalNotification(notification.Title, notification.Body);
    }

    private void ShowLocalNotification(string? title, string? body)
    {
        var notificationManager = NotificationManagerCompat.From(this);

        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            var channel = new NotificationChannel(ChannelId, "CineLog", NotificationImportance.Default);
            ((NotificationManager)GetSystemService(NotificationService)!).CreateNotificationChannel(channel);
        }

        var notification = new NotificationCompat.Builder(this, ChannelId)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentTitle(title)
            .SetContentText(body)
            .SetAutoCancel(true)
            .Build();

        notificationManager.Notify(Environment.TickCount, notification);
    }
}
