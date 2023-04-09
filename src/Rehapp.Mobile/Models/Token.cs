using System.Text.Json.Serialization;

namespace Rehapp.Mobile.Models;

public class Token
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = null!;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = null!;
}
