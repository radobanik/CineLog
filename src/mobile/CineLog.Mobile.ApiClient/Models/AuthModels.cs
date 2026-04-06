// AUTO-GENERATED — run CineLog.ApiClientGenerator to regenerate.
namespace CineLog.Mobile.ApiClient.Models;

public class AuthResponse
{
    [Newtonsoft.Json.JsonProperty("token")]
    public string Token { get; set; } = null!;

    [Newtonsoft.Json.JsonProperty("userId")]
    public Guid UserId { get; set; }

    [Newtonsoft.Json.JsonProperty("username")]
    public string Username { get; set; } = null!;
}

public class LoginCommand
{
    [Newtonsoft.Json.JsonProperty("email")]
    public string Email { get; set; } = null!;

    [Newtonsoft.Json.JsonProperty("password")]
    public string Password { get; set; } = null!;
}

public class RegisterCommand
{
    [Newtonsoft.Json.JsonProperty("username")]
    public string Username { get; set; } = null!;

    [Newtonsoft.Json.JsonProperty("email")]
    public string Email { get; set; } = null!;

    [Newtonsoft.Json.JsonProperty("password")]
    public string Password { get; set; } = null!;
}
