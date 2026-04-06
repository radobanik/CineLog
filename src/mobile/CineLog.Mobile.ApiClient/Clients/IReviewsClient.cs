using CineLog.Mobile.ApiClient.Infrastructure;
using CineLog.Mobile.ApiClient.Models;

namespace CineLog.Mobile.ApiClient.Clients;

[System.CodeDom.Compiler.GeneratedCode("NSwag", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial interface IReviewsClient
{
    /// <summary>
    /// Get a review by id.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<ReviewResponse> ReviewsGET2Async(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get a review by id.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<ReviewResponse> ReviewsGET2Async(System.Guid id, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Update an existing review.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<ReviewResponse> ReviewsPUTAsync(System.Guid id, UpdateReviewCommand? body);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Update an existing review.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<ReviewResponse> ReviewsPUTAsync(System.Guid id, UpdateReviewCommand? body, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Delete a review.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task ReviewsDELETEAsync(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Delete a review.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task ReviewsDELETEAsync(System.Guid id, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Create a review for a movie.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<ReviewResponse> ReviewsPOSTAsync(CreateReviewCommand? body);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Create a review for a movie.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<ReviewResponse> ReviewsPOSTAsync(CreateReviewCommand? body, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Toggle like on a review.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task ToggleLikeAsync(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Toggle like on a review.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task ToggleLikeAsync(System.Guid id, System.Threading.CancellationToken cancellationToken);
}
