using CineLog.Mobile.Core.Models.Activity;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IActivityFeedService
{
    Task<IReadOnlyList<ActivityFeedItem>> GetActivityFeedAsync(
        int count = 50,
        CancellationToken ct = default);
}
