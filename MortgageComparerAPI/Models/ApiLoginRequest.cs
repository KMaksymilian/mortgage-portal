using System.Text.Json.Serialization;

namespace MortgageComparerAPI.Models;

public class ApiLoginRequest
{
    [JsonPropertyName("clientSecret")]
    public string ClientSecret { get; set; }

    public ApiLoginRequest(string clientSecret)
    {
        ClientSecret = clientSecret;
    }
}