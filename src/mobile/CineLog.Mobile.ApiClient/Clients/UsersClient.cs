using CineLog.Mobile.ApiClient.Infrastructure;
using CineLog.Mobile.ApiClient.Models;

namespace CineLog.Mobile.ApiClient.Clients;

[System.CodeDom.Compiler.GeneratedCode("NSwag", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class UsersClient : IUsersClient
{
    private System.Net.Http.HttpClient _httpClient;
    private static System.Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(CreateSerializerSettings, true);
    private Newtonsoft.Json.JsonSerializerSettings _instanceSettings;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public UsersClient(System.Net.Http.HttpClient httpClient)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    {
        _httpClient = httpClient;
        Initialize();
    }

    private static Newtonsoft.Json.JsonSerializerSettings CreateSerializerSettings()
    {
        var settings = new Newtonsoft.Json.JsonSerializerSettings();
        UpdateJsonSerializerSettings(settings);
        return settings;
    }

    protected Newtonsoft.Json.JsonSerializerSettings JsonSerializerSettings
    {
        get
        {
            return _instanceSettings ?? _settings.Value;
        }
    }

    static partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings);
    partial void Initialize();
    partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url);
    partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder);
    partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response);
    /// <summary>
    /// Get the current user's profile.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task<UserProfileResponse> MeGETAsync()
    {
        return MeGETAsync(System.Threading.CancellationToken.None);
    }

    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get the current user's profile.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<UserProfileResponse> MeGETAsync(System.Threading.CancellationToken cancellationToken)
    {
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/plain"));
                var urlBuilder_ = new System.Text.StringBuilder();
                // Operation Path: "api/users/me"
                urlBuilder_.Append("api/users/me");
                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);
                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);
                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<UserProfileResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        return objectResponse_.Object;
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    /// <summary>
    /// Update the current user's profile.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task<UserProfileResponse> MePUTAsync(UpdateProfileCommand? body)
    {
        return MePUTAsync(body, System.Threading.CancellationToken.None);
    }

    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Update the current user's profile.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<UserProfileResponse> MePUTAsync(UpdateProfileCommand? body, System.Threading.CancellationToken cancellationToken)
    {
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var json_ = Newtonsoft.Json.JsonConvert.SerializeObject(body, JsonSerializerSettings);
                var content_ = new System.Net.Http.StringContent(json_);
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("PUT");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/plain"));
                var urlBuilder_ = new System.Text.StringBuilder();
                // Operation Path: "api/users/me"
                urlBuilder_.Append("api/users/me");
                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);
                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);
                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<UserProfileResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        return objectResponse_.Object;
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    /// <summary>
    /// Get user profile by id.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task<UserProfileResponse> UsersAsync(System.Guid id)
    {
        return UsersAsync(id, System.Threading.CancellationToken.None);
    }

    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get user profile by id.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<UserProfileResponse> UsersAsync(System.Guid id, System.Threading.CancellationToken cancellationToken)
    {
        if (id == null)
            throw new System.ArgumentNullException("id");
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/plain"));
                var urlBuilder_ = new System.Text.StringBuilder();
                // Operation Path: "api/users/{id}"
                urlBuilder_.Append("api/users/");
                urlBuilder_.Append(System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);
                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);
                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<UserProfileResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        return objectResponse_.Object;
                    }
                    else if (status_ == 404)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ProblemDetails>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        throw new ApiException<ProblemDetails>("Not Found", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    /// <summary>
    /// Get a user's reviews.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task<ReviewResponsePagedResponse> ReviewsGET3Async(System.Guid id, int? page, int? pageSize)
    {
        return ReviewsGET3Async(id, page, pageSize, System.Threading.CancellationToken.None);
    }

    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get a user's reviews.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<ReviewResponsePagedResponse> ReviewsGET3Async(System.Guid id, int? page, int? pageSize, System.Threading.CancellationToken cancellationToken)
    {
        if (id == null)
            throw new System.ArgumentNullException("id");
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/plain"));
                var urlBuilder_ = new System.Text.StringBuilder();
                // Operation Path: "api/users/{id}/reviews"
                urlBuilder_.Append("api/users/");
                urlBuilder_.Append(System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
                urlBuilder_.Append("/reviews");
                urlBuilder_.Append('?');
                if (page != null)
                {
                    urlBuilder_.Append(System.Uri.EscapeDataString("page")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(page, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                }

                if (pageSize != null)
                {
                    urlBuilder_.Append(System.Uri.EscapeDataString("pageSize")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(pageSize, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                }

                urlBuilder_.Length--;
                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);
                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);
                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ReviewResponsePagedResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        return objectResponse_.Object;
                    }
                    else if (status_ == 404)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ProblemDetails>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        throw new ApiException<ProblemDetails>("Not Found", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    /// <summary>
    /// Get a user's followers.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task<UserSummaryResponsePagedResponse> FollowersAsync(System.Guid id, int? page, int? pageSize)
    {
        return FollowersAsync(id, page, pageSize, System.Threading.CancellationToken.None);
    }

    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get a user's followers.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<UserSummaryResponsePagedResponse> FollowersAsync(System.Guid id, int? page, int? pageSize, System.Threading.CancellationToken cancellationToken)
    {
        if (id == null)
            throw new System.ArgumentNullException("id");
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/plain"));
                var urlBuilder_ = new System.Text.StringBuilder();
                // Operation Path: "api/users/{id}/followers"
                urlBuilder_.Append("api/users/");
                urlBuilder_.Append(System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
                urlBuilder_.Append("/followers");
                urlBuilder_.Append('?');
                if (page != null)
                {
                    urlBuilder_.Append(System.Uri.EscapeDataString("page")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(page, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                }

                if (pageSize != null)
                {
                    urlBuilder_.Append(System.Uri.EscapeDataString("pageSize")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(pageSize, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                }

                urlBuilder_.Length--;
                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);
                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);
                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<UserSummaryResponsePagedResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        return objectResponse_.Object;
                    }
                    else if (status_ == 404)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ProblemDetails>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        throw new ApiException<ProblemDetails>("Not Found", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    /// <summary>
    /// Get users that a user is following.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task<UserSummaryResponsePagedResponse> FollowingAsync(System.Guid id, int? page, int? pageSize)
    {
        return FollowingAsync(id, page, pageSize, System.Threading.CancellationToken.None);
    }

    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get users that a user is following.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<UserSummaryResponsePagedResponse> FollowingAsync(System.Guid id, int? page, int? pageSize, System.Threading.CancellationToken cancellationToken)
    {
        if (id == null)
            throw new System.ArgumentNullException("id");
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/plain"));
                var urlBuilder_ = new System.Text.StringBuilder();
                // Operation Path: "api/users/{id}/following"
                urlBuilder_.Append("api/users/");
                urlBuilder_.Append(System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
                urlBuilder_.Append("/following");
                urlBuilder_.Append('?');
                if (page != null)
                {
                    urlBuilder_.Append(System.Uri.EscapeDataString("page")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(page, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                }

                if (pageSize != null)
                {
                    urlBuilder_.Append(System.Uri.EscapeDataString("pageSize")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(pageSize, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                }

                urlBuilder_.Length--;
                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);
                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);
                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<UserSummaryResponsePagedResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        return objectResponse_.Object;
                    }
                    else if (status_ == 404)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ProblemDetails>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        throw new ApiException<ProblemDetails>("Not Found", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    /// <summary>
    /// Get current user's favorites.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task<System.Collections.Generic.ICollection<MovieListItemResponse>> FavoritesAllAsync()
    {
        return FavoritesAllAsync(System.Threading.CancellationToken.None);
    }

    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Get current user's favorites.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<System.Collections.Generic.ICollection<MovieListItemResponse>> FavoritesAllAsync(System.Threading.CancellationToken cancellationToken)
    {
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/plain"));
                var urlBuilder_ = new System.Text.StringBuilder();
                // Operation Path: "api/users/me/favorites"
                urlBuilder_.Append("api/users/me/favorites");
                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);
                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);
                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<System.Collections.Generic.ICollection<MovieListItemResponse>>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        return objectResponse_.Object;
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    /// <summary>
    /// Upload or replace the current user's avatar.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task<UploadAvatarResponse> AvatarAsync(FileParameter file)
    {
        return AvatarAsync(file, System.Threading.CancellationToken.None);
    }

    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Upload or replace the current user's avatar.
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<UploadAvatarResponse> AvatarAsync(FileParameter file, System.Threading.CancellationToken cancellationToken)
    {
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var boundary_ = System.Guid.NewGuid().ToString();
                var content_ = new System.Net.Http.MultipartFormDataContent(boundary_);
                content_.Headers.Remove("Content-Type");
                content_.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary_);
                if (file == null)
                    throw new System.ArgumentNullException("file");
                else
                {
                    var content_file_ = new System.Net.Http.StreamContent(file.Data);
                    if (!string.IsNullOrEmpty(file.ContentType))
                        content_file_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(file.ContentType);
                    content_.Add(content_file_, "file", file.FileName ?? "file");
                }

                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("PUT");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/plain"));
                var urlBuilder_ = new System.Text.StringBuilder();
                // Operation Path: "api/users/me/avatar"
                urlBuilder_.Append("api/users/me/avatar");
                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);
                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);
                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<UploadAvatarResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        return objectResponse_.Object;
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    /// <summary>
    /// Set avatar for any user (admin only).
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task<UploadAvatarResponse> Avatar2Async(System.Guid id, FileParameter file)
    {
        return Avatar2Async(id, file, System.Threading.CancellationToken.None);
    }

    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Set avatar for any user (admin only).
    /// </summary>
    /// <returns>OK</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<UploadAvatarResponse> Avatar2Async(System.Guid id, FileParameter file, System.Threading.CancellationToken cancellationToken)
    {
        if (id == null)
            throw new System.ArgumentNullException("id");
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var boundary_ = System.Guid.NewGuid().ToString();
                var content_ = new System.Net.Http.MultipartFormDataContent(boundary_);
                content_.Headers.Remove("Content-Type");
                content_.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary_);
                if (file == null)
                    throw new System.ArgumentNullException("file");
                else
                {
                    var content_file_ = new System.Net.Http.StreamContent(file.Data);
                    if (!string.IsNullOrEmpty(file.ContentType))
                        content_file_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(file.ContentType);
                    content_.Add(content_file_, "file", file.FileName ?? "file");
                }

                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("PUT");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("text/plain"));
                var urlBuilder_ = new System.Text.StringBuilder();
                // Operation Path: "api/users/{id}/avatar"
                urlBuilder_.Append("api/users/");
                urlBuilder_.Append(System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
                urlBuilder_.Append("/avatar");
                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);
                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);
                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<UploadAvatarResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        return objectResponse_.Object;
                    }
                    else if (status_ == 404)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ProblemDetails>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        throw new ApiException<ProblemDetails>("Not Found", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    /// <summary>
    /// Follow a user.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task FollowPOSTAsync(System.Guid id)
    {
        return FollowPOSTAsync(id, System.Threading.CancellationToken.None);
    }

    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Follow a user.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task FollowPOSTAsync(System.Guid id, System.Threading.CancellationToken cancellationToken)
    {
        if (id == null)
            throw new System.ArgumentNullException("id");
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                request_.Method = new System.Net.Http.HttpMethod("POST");
                var urlBuilder_ = new System.Text.StringBuilder();
                // Operation Path: "api/users/{id}/follow"
                urlBuilder_.Append("api/users/");
                urlBuilder_.Append(System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
                urlBuilder_.Append("/follow");
                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);
                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);
                    var status_ = (int)response_.StatusCode;
                    if (status_ == 204)
                    {
                        return;
                    }
                    else if (status_ == 404)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ProblemDetails>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        throw new ApiException<ProblemDetails>("Not Found", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                    else if (status_ == 409)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ProblemDetails>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        throw new ApiException<ProblemDetails>("Conflict", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    /// <summary>
    /// Unfollow a user.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual System.Threading.Tasks.Task FollowDELETEAsync(System.Guid id)
    {
        return FollowDELETEAsync(id, System.Threading.CancellationToken.None);
    }

    /// <param name = "cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Unfollow a user.
    /// </summary>
    /// <returns>No Content</returns>
    /// <exception cref = "ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task FollowDELETEAsync(System.Guid id, System.Threading.CancellationToken cancellationToken)
    {
        if (id == null)
            throw new System.ArgumentNullException("id");
        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("DELETE");
                var urlBuilder_ = new System.Text.StringBuilder();
                // Operation Path: "api/users/{id}/follow"
                urlBuilder_.Append("api/users/");
                urlBuilder_.Append(System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
                urlBuilder_.Append("/follow");
                PrepareRequest(client_, request_, urlBuilder_);
                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                PrepareRequest(client_, request_, url_);
                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                    foreach (var item_ in response_.Headers)
                        headers_[item_.Key] = item_.Value;
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);
                    var status_ = (int)response_.StatusCode;
                    if (status_ == 204)
                    {
                        return;
                    }
                    else if (status_ == 404)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ProblemDetails>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }

                        throw new ApiException<ProblemDetails>("Not Found", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                    else
                    {
                        var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    protected struct ObjectResponseResult<T>
    {
        public ObjectResponseResult(T responseObject, string responseText)
        {
            this.Object = responseObject;
            this.Text = responseText;
        }

        public T Object { get; }
        public string Text { get; }
    }

    public bool ReadResponseAsString { get; set; }

    protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
    {
        if (response == null || response.Content == null)
        {
            return new ObjectResponseResult<T>(default(T)!, string.Empty);
        }

        if (ReadResponseAsString)
        {
            var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText, JsonSerializerSettings);
                return new ObjectResponseResult<T>(typedBody!, responseText);
            }
            catch (Newtonsoft.Json.JsonException exception)
            {
                var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
            }
        }
        else
        {
            try
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new System.IO.StreamReader(responseStream))
                using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                {
                    var serializer = Newtonsoft.Json.JsonSerializer.Create(JsonSerializerSettings);
                    var typedBody = serializer.Deserialize<T>(jsonTextReader);
                    return new ObjectResponseResult<T>(typedBody!, string.Empty);
                }
            }
            catch (Newtonsoft.Json.JsonException exception)
            {
                var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
            }
        }
    }

    private string ConvertToString(object? value, System.Globalization.CultureInfo cultureInfo)
    {
        if (value == null)
        {
            return "";
        }

        if (value is System.Enum)
        {
            var name = System.Enum.GetName(value.GetType(), value);
            if (name != null)
            {
                var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                if (field != null)
                {
                    var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) as System.Runtime.Serialization.EnumMemberAttribute;
                    if (attribute != null)
                    {
                        return attribute.Value != null ? attribute.Value : name;
                    }
                }

                var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                return converted == null ? string.Empty : converted;
            }
        }
        else if (value is bool)
        {
            return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
        }
        else if (value is byte[])
        {
            return System.Convert.ToBase64String((byte[])value);
        }
        else if (value is string[])
        {
            return string.Join(",", (string[])value);
        }
        else if (value.GetType().IsArray)
        {
            var valueArray = (System.Array)value;
            var valueTextArray = new string[valueArray.Length];
            for (var i = 0; i < valueArray.Length; i++)
            {
                valueTextArray[i] = ConvertToString(valueArray.GetValue(i), cultureInfo);
            }

            return string.Join(",", valueTextArray);
        }

        var result = System.Convert.ToString(value, cultureInfo);
        return result == null ? "" : result;
    }
}
