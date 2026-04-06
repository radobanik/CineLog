
namespace CineLog.Mobile.ApiClient.Models;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class CreateReviewCommand
{
    [Newtonsoft.Json.JsonProperty("movieId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Guid? MovieId { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("rating", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public double? Rating { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("reviewText", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? ReviewText { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("containsSpoilers", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public bool? ContainsSpoilers { get; set; } = default!;
}
