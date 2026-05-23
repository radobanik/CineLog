using CineLog.Mobile.Core.Models.Activity;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IActivityFeedService
{
    Task<IReadOnlyList<ActivityFeedItem>> GetActivityFeedAsync(
        int skip = 0,
        int count = 25,
        CancellationToken ct = default);
}
