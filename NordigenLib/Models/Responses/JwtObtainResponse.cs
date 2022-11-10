using Newtonsoft.Json;

namespace NordigenLib.Responses;

[JsonObject]
public class JwtObtainResponse {
    [JsonIgnore] private DateTime _dateCreated = DateTime.Now;

    /// <summary>
    /// Access token received from Nordigen
    /// </summary>
    [JsonProperty("access")]
    public required string AccessToken { get; set; }

    /// <summary>
    /// Number of seconds before the access token expires
    /// </summary>
    [JsonProperty("access_expires")]
    public required int AccessExpires { get; set; }

    /// <summary>
    /// Refresh token received from Nordigen
    /// </summary>
    [JsonProperty("refresh")]
    public required string RefreshToken { get; set; }

    /// <summary>
    /// Number of seconds before the refresh token expires
    /// </summary>
    [JsonProperty("refresh_expires")]
    public required int RefreshExpires { get; set; }

    [JsonIgnore] public DateTime AccessExpiresDateTime => _dateCreated.AddSeconds(AccessExpires);
}