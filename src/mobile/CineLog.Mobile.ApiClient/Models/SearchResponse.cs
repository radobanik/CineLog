
namespace CineLog.Mobile.ApiClient.Models;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class SearchResponse
{
    [Newtonsoft.Json.JsonProperty("movies", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Collections.Generic.ICollection<MovieSummaryResponse>? Movies { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("totalMovies", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? TotalMovies { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("people", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Collections.Generic.ICollection<PersonSummaryResponse>? People { get; set; } = default !;

    [Newtonsoft.Json.JsonProperty("totalPeople", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? TotalPeople { get; set; } = default !;
}
