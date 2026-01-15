using System.Text.Json.Serialization;

namespace MortgageComparerAPI.Models;

public class ApiLoginResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }
    [JsonPropertyName("expiresIn")]
    public string ExpiresIn { get; set; }
}