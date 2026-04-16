using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.ApiClient.Infrastructure;

namespace CineLog.Mobile.ApiClient.Clients;

[System.CodeDom.Compiler.GeneratedCode("NSwag", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial interface IWatchlistsClient
{
    /// <summary>
    /// Get all watchlists for the current user.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<WatchlistSummaryResponse>> GetAllAsync();
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get all watchlists for the current user.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<WatchlistSummaryResponse>> GetAllAsync(System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Create a new watchlist.
    /// </summary>
    /// <returns>Created</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Guid> CreateAsync(CreateWatchlistCommand? body);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Create a new watchlist.
    /// </summary>
    /// <returns>Created</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Guid> CreateAsync(CreateWatchlistCommand? body, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Get a watchlist with its movies.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<WatchlistDetailResponse> GetByIdAsync(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get a watchlist with its movies.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<WatchlistDetailResponse> GetByIdAsync(System.Guid id, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Delete a watchlist.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task DeleteAsync(System.Guid id);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Delete a watchlist.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task DeleteAsync(System.Guid id, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Add a movie to a watchlist.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task AddMovieAsync(System.Guid id, System.Guid movieId);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Add a movie to a watchlist.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task AddMovieAsync(System.Guid id, System.Guid movieId, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Remove a movie from a watchlist.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task RemoveMovieAsync(System.Guid id, System.Guid movieId);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Remove a movie from a watchlist.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task RemoveMovieAsync(System.Guid id, System.Guid movieId, System.Threading.CancellationToken cancellationToken);
}
