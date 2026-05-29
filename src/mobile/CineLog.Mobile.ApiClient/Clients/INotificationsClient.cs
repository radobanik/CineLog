using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.ApiClient.Infrastructure;

namespace CineLog.Mobile.ApiClient.Clients;

[System.CodeDom.Compiler.GeneratedCode("NSwag", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial interface INotificationsClient
{
    /// <summary>
    /// Register or update the FCM token for push notifications.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task RegisterFcmTokenAsync(RegisterFcmTokenCommand? body);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Register or update the FCM token for push notifications.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task RegisterFcmTokenAsync(RegisterFcmTokenCommand? body, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Send a test push notification to the current user.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task SendTestAsync(SendTestNotificationCommand? body);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Send a test push notification to the current user.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task SendTestAsync(SendTestNotificationCommand? body, System.Threading.CancellationToken cancellationToken);
}
