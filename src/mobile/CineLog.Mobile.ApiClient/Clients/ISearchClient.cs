using CineLog.Mobile.ApiClient.Infrastructure;
using CineLog.Mobile.ApiClient.Models;

namespace CineLog.Mobile.ApiClient.Clients;

[System.CodeDom.Compiler.GeneratedCode("NSwag", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial interface ISearchClient
{
    /// <summary>
    /// Search movies and people by a single query.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<SearchResponse> SearchAsync(string? query, int? page, int? pageSize);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Search movies and people by a single query.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<SearchResponse> SearchAsync(string? query, int? page, int? pageSize, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Search movies, optionally filtered by genres.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<MovieSummaryResponsePagedResponse> MoviesGET2Async(string? query, System.Collections.Generic.IEnumerable<string>? genres, int? page, int? pageSize);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Search movies, optionally filtered by genres.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<MovieSummaryResponsePagedResponse> MoviesGET2Async(string? query, System.Collections.Generic.IEnumerable<string>? genres, int? page, int? pageSize, System.Threading.CancellationToken cancellationToken);
    /// <summary>
    /// Search people.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<PersonSummaryResponsePagedResponse> PeopleGET2Async(string? query, int? page, int? pageSize);
    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Search people.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    System.Threading.Tasks.Task<PersonSummaryResponsePagedResponse> PeopleGET2Async(string? query, int? page, int? pageSize, System.Threading.CancellationToken cancellationToken);
}
