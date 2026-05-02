using System.Net.Http.Json;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Search;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class SearchService(ISearchClient searchClient) : ISearchService
{
    private const int PageSize = 12;

    public async Task<(IReadOnlyList<MovieItem> Movies, bool HasMore)> SearchMoviesAsync(
        string query,
        int page,
        CancellationToken ct = default)
    {
        var result = await searchClient.SearchMoviesAsync(query, genres: null, page, PageSize, ct);

        var movies = result.Items is null
            ? (IReadOnlyList<MovieItem>)[]
            : [.. result.Items.Select(m => new MovieItem
            {
                Id = m.Id ?? Guid.Empty,
                Title = m.Title ?? string.Empty,
                PosterPath = m.PosterPath,
                AverageRating = (double?)m.AverageRating
            })];

        var hasMore = page < (result.TotalPages ?? 1);

        return (movies, hasMore);
    }

    public async Task<(IReadOnlyList<UserSearchItem> Users, bool HasMore)> SearchUsersAsync(
         string query,
         int page,
         CancellationToken ct = default)
    {
        var result = await searchClient.SearchUsersAsync(query, page, PageSize, ct);

        return (
            MapUsers(result.Items),
            page < (result.TotalPages ?? 1));
    }

    private static IReadOnlyList<UserSearchItem> MapUsers(
        IEnumerable<DiscoverUserResponse>? users)
    {
        return users?.Select(u => new UserSearchItem
        {
            Id = u.Id ?? Guid.Empty,
            Username = u.Username ?? string.Empty,
            AvatarUrl = u.AvatarUrl,
            ReviewCount = u.ReviewCount ?? 0,
            IsFollowing = u.IsFollowing ?? false
        }).ToList() ?? [];
    }

}
