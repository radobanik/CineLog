
namespace CineLog.Mobile.ApiClient.Models;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class UserProfileResponse
{
    [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Guid? Id { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("username", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? Username { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("bio", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? Bio { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("avatarUrl", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? AvatarUrl { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("filmsCount", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? FilmsCount { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("followersCount", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? FollowersCount { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("followingCount", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? FollowingCount { get; set; } = default!;
}
