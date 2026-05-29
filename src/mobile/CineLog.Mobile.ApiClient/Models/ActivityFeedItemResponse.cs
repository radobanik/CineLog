
namespace CineLog.Mobile.ApiClient.Models;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class ActivityFeedItemResponse
{
    [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Guid? Id { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public ActivityType? Type { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("createdAt", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.DateTimeOffset? CreatedAt { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("actor", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public ActivityUserInfo? Actor { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("targetUser", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public ActivityUserInfo? TargetUser { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("movie", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public ActivityMovieInfo? Movie { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("review", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public ActivityReviewInfo? Review { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("watchlist", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public ActivityWatchlistInfo? Watchlist { get; set; } = default !;
}
