using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;

namespace BasicTests;

public class UnitTest1
{
    private readonly IConfiguration _configuration;
    public UnitTest1()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<UnitTest1>()
            .AddEnvironmentVariables();

        _configuration = builder.Build();
    }
    [Fact]
    public void AlwaysTrueTest()
    {
        Assert.True(true);
    }

    // basic czy w ogóle działa jakiekolwiek połączenie z api
    [Fact]
    public async Task ExternalApiCallShouldReturnCode200Async()
    {
        var tokenUrl = "https://indentitymanager.snet.com.pl/connect/token";
        var clientId = _configuration["ExternalApi:Login"];
        var clientSecret = _configuration["ExternalApi:Secret"];
        
        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "scope", "MiNI.LoanBank.API" }
        };
        var content = new FormUrlEncodedContent(requestData);
        var client = new HttpClient();
        var tokenResponse = await client.PostAsync(tokenUrl, content);
        
        if (!tokenResponse.IsSuccessStatusCode)
        {
            var errorContent = await tokenResponse.Content.ReadAsStringAsync();
            Assert.Fail($"Nie udało się pobrać tokena. Kod: {tokenResponse.StatusCode}. Treść: {errorContent}");
        }

        var tokenDto = await tokenResponse.Content.ReadFromJsonAsync<TokenResponseDto>();

        Assert.NotNull(tokenDto);
        Assert.False(string.IsNullOrEmpty(tokenDto.AccessToken), "Pobrano token, ale pole AccessToken jest puste (błąd mapowania JSON).");
        
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);

        var result = await client.GetAsync("https://mini.loanbank.api.snet.com.pl/api/identity/claims");
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
    
    /*[Fact]
    public async Task ExternalApiCallShouldReturn*/
    private class TokenResponseDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }


}
