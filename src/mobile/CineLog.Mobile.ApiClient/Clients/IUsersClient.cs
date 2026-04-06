using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.ApiClient.Infrastructure;

namespace CineLog.Mobile.ApiClient.Clients;

[System.CodeDom.Compiler.GeneratedCode("NSwag", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial interface IUsersClient
{
    /// <summary>
    /// Get the current user's profile.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserProfileResponse> MeGETAsync();
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get the current user's profile.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserProfileResponse> MeGETAsync(System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Update the current user's profile.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserProfileResponse> MePUTAsync(UpdateProfileCommand? body);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Update the current user's profile.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserProfileResponse> MePUTAsync(UpdateProfileCommand? body, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Get user profile by id.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserProfileResponse> UsersAsync(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get user profile by id.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserProfileResponse> UsersAsync(System.Guid id, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Get a user's reviews.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<ReviewResponsePagedResponse> ReviewsGET3Async(System.Guid id, int? page, int? pageSize);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get a user's reviews.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<ReviewResponsePagedResponse> ReviewsGET3Async(System.Guid id, int? page, int? pageSize, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Get a user's followers.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserSummaryResponsePagedResponse> FollowersAsync(System.Guid id, int? page, int? pageSize);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get a user's followers.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserSummaryResponsePagedResponse> FollowersAsync(System.Guid id, int? page, int? pageSize, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Get users that a user is following.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserSummaryResponsePagedResponse> FollowingAsync(System.Guid id, int? page, int? pageSize);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get users that a user is following.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UserSummaryResponsePagedResponse> FollowingAsync(System.Guid id, int? page, int? pageSize, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Get current user's favorites.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<MovieListItemResponse>> FavoritesAllAsync();
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get current user's favorites.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<MovieListItemResponse>> FavoritesAllAsync(System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Upload or replace the current user's avatar.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UploadAvatarResponse> AvatarAsync(FileParameter file);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Upload or replace the current user's avatar.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UploadAvatarResponse> AvatarAsync(FileParameter file, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Set avatar for any user (admin only).
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UploadAvatarResponse> Avatar2Async(System.Guid id, FileParameter file);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Set avatar for any user (admin only).
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<UploadAvatarResponse> Avatar2Async(System.Guid id, FileParameter file, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Follow a user.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task FollowPOSTAsync(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Follow a user.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task FollowPOSTAsync(System.Guid id, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Unfollow a user.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task FollowDELETEAsync(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Unfollow a user.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task FollowDELETEAsync(System.Guid id, System.Threading.CancellationToken cancellationToken);
}
