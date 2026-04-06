
namespace CineLog.Mobile.ApiClient.Models;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class MovieDetailResponse
{
    [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Guid? Id { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("idTmdb", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? IdTmdb { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public MovieType? Type { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("title", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? Title { get; set; } = default !;

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

    [Newtonsoft.Json.JsonProperty("averageRating", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public double? AverageRating { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("ratingsCount", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? RatingsCount { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("genres", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Collections.Generic.ICollection<GenreResponse>? Genres { get; set; } = default !;

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

    [Newtonsoft.Json.JsonProperty("popularity", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public double? Popularity { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("numberOfSeasons", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? NumberOfSeasons { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("numberOfEpisodes", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? NumberOfEpisodes { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("cast", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Collections.Generic.ICollection<CastMemberResponse>? Cast { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("crew", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Collections.Generic.ICollection<CrewMemberResponse>? Crew { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("productionCompanies", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Collections.Generic.ICollection<ProductionCompanyResponse>? ProductionCompanies { get; set; } = default !;
}
