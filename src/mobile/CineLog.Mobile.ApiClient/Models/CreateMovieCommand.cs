
namespace CineLog.Mobile.ApiClient.Models;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class CreateMovieCommand
{
    [Newtonsoft.Json.JsonProperty("tmdbId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? TmdbId { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("title", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? Title { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public MovieType? Type { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("overview", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? Overview { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("posterPath", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? PosterPath { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("backdropPath", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? BackdropPath { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("releaseDate", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    [Newtonsoft.Json.JsonConverter(typeof(DateFormatConverter))]
    public System.DateTimeOffset? ReleaseDate { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("runtimeMinutes", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? RuntimeMinutes { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("imdbId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? ImdbId { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("originalTitle", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? OriginalTitle { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("originalLanguage", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? OriginalLanguage { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("tagline", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? Tagline { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? Status { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("budget", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public long? Budget { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("revenue", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public double? Revenue { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("numberOfSeasons", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? NumberOfSeasons { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("numberOfEpisodes", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? NumberOfEpisodes { get; set; } = default !;
}
