using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.ApiClient.Infrastructure;

namespace CineLog.Mobile.ApiClient.Clients;

[System.CodeDom.Compiler.GeneratedCode("NSwag", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial interface IMoviesClient
{
    /// <summary>
    /// Create a new movie.
    /// </summary>
    /// <returns>Created</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Guid> CreateAsync(CreateMovieCommand? body);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Create a new movie.
    /// </summary>
    /// <returns>Created</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Guid> CreateAsync(CreateMovieCommand? body, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Update a movie.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task UpdateAsync(System.Guid id, UpdateMovieCommand? body);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Update a movie.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task UpdateAsync(System.Guid id, UpdateMovieCommand? body, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Get movie detail by id.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<MovieDetailResponse> GetByIdAsync(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get movie detail by id.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<MovieDetailResponse> GetByIdAsync(System.Guid id, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Delete a movie and all its reviews.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task DeleteAsync(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Delete a movie and all its reviews.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task DeleteAsync(System.Guid id, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Get reviews for a movie.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<ReviewResponsePagedResponse> GetReviewsAsync(System.Guid movieId, int? page, int? pageSize);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get reviews for a movie.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<ReviewResponsePagedResponse> GetReviewsAsync(System.Guid movieId, int? page, int? pageSize, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Add movie to current user's favorites.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task AddToFavoritesAsync(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Add movie to current user's favorites.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task AddToFavoritesAsync(System.Guid id, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Remove movie from current user's favorites.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task RemoveFromFavoritesAsync(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Remove movie from current user's favorites.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task RemoveFromFavoritesAsync(System.Guid id, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Set poster image for a movie.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<SetMovieImageResponse> SetPosterAsync(System.Guid id, FileParameter file);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Set poster image for a movie.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<SetMovieImageResponse> SetPosterAsync(System.Guid id, FileParameter file, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Set backdrop image for a movie.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<SetMovieImageResponse> SetBackdropAsync(System.Guid id, FileParameter file);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Set backdrop image for a movie.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<SetMovieImageResponse> SetBackdropAsync(System.Guid id, FileParameter file, System.Threading.CancellationToken cancellationToken);
}
