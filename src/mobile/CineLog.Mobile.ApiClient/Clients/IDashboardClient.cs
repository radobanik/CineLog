using CineLog.Mobile.ApiClient.Infrastructure;
using CineLog.Mobile.ApiClient.Models;

namespace CineLog.Mobile.ApiClient.Clients;

[System.CodeDom.Compiler.GeneratedCode("NSwag", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial interface IDashboardClient
{
    /// <summary>
    /// Get top-rated movies.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<MovieSummaryResponse>> TopRatedMoviesAsync(int? count);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get top-rated movies.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<MovieSummaryResponse>> TopRatedMoviesAsync(int? count, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Get the latest review activity across all users.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<NewActionResponse>> NewActionsAsync(int? count);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get the latest review activity across all users.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<NewActionResponse>> NewActionsAsync(int? count, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Get the newest movies by release date.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<MovieSummaryResponse>> NewestMoviesAsync(int? count);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get the newest movies by release date.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<System.Collections.Generic.ICollection<MovieSummaryResponse>> NewestMoviesAsync(int? count, System.Threading.CancellationToken cancellationToken);
}
