using System.Text.Json;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services.Interfaces;
using JobTypeEntity = MortgageComparer.Entities.JobTypeEntity;

namespace MortgageComparer.Services;

public class ExternalApiService : IExternalApiService
{
    private IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    
    public ExternalApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }
    public async Task<JobTypeEntity> GetJobTypesAsync()
    {
        var tokenDto =  await GetTokenAsync();
        using var client = new HttpClient();
        var jobApiResponse = 
            await client.GetAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Dictionary/jobTypes");

        var jobJsonResponse = await jobApiResponse.Content.ReadAsStringAsync();
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var jobResult = JsonSerializer.Deserialize<List<JobTypeEntity>>(jobJsonResponse, jsonSettings);
        int count = jobResult.Count;
        int randomNumber = new Random().Next(0, count);
        JobTypeEntity userJob = new JobTypeEntity
        {
            JobTypeId = jobResult[randomNumber].JobTypeId,
            Name = jobResult[randomNumber].Name,
            Description = jobResult[randomNumber].Description
        };
        return userJob;
    }
    public async Task<PersonalDocumentTypeEntity> GetDocumentTypesAsync()
    {
        var tokenDto =  await GetTokenAsync();
        using var client = new HttpClient();
        var documentApiResponse = 
            await client.GetAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Dictionary/governmentDocumentTypes");

        var documentJsonResponse = await documentApiResponse.Content.ReadAsStringAsync();
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var documentResult = JsonSerializer.Deserialize<List<PersonalDocumentTypeEntity>>(documentJsonResponse, jsonSettings);
        int count = documentResult.Count;
        int randomNumber = new Random().Next(0, count);
        PersonalDocumentTypeEntity userJob = new PersonalDocumentTypeEntity()
        {
            PersonalDocumentId = documentResult[randomNumber].PersonalDocumentId,
            Name = documentResult[randomNumber].Name,
            Description = documentResult[randomNumber].Description
        };
        return userJob;
    }
    public async Task<string?> GetTokenAsync()
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
        using var client = new HttpClient();
        var response =  await client.PostAsync(tokenUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        return token?.AccessToken;
    }
}