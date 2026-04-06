
namespace CineLog.Mobile.ApiClient.Models;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class UpdateProfileCommand
{
    [Newtonsoft.Json.JsonProperty("bio", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? Bio { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("avatarUrl", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? AvatarUrl { get; set; } = default!;
}
