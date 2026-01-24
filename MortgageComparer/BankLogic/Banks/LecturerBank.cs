using Azure.Core;
using MortgageComparer.BankProviders;
using MortgageComparer.DataTransferObjects;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparerAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MortgageComparer.BankLogic.Banks;

public class LecturerBank : IBankService
{
    private IConfiguration _configuration;
    private HttpClient _client;
    public string Name { get; } =  "LecturerBank";

    public LecturerBank(IHttpClientFactory clientFactory,  IConfiguration configuration)
    {
        _client = clientFactory.CreateClient("LecturerBankApi");
        _configuration = configuration;
    }

    public async Task<string?> GetTokenAsync()
    {
        var tokenUrl = "https://indentitymanager.snet.com.pl/connect/token";
        var clientId = _configuration["ExternalApi:Login"];
        var clientSecret = _configuration["ExternalApi:Secret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new Exception("Brak konfiguracji dla poświadczeń API banku.");
        }

        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "scope", "MiNI.LoanBank.API" }
        };
        var content = new FormUrlEncodedContent(requestData);
        var response =  await _client.PostAsync(tokenUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        return token?.AccessToken;
    }

    public async Task<QuoteDto> PostQuoteAsync(QuoteDto quoteDto) {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var quoteRequest = new {
            quoteDto.RequestedAmount,
            quoteDto.InstalmentNumber
        };

        var apiResponse = await _client.PostAsJsonAsync("v1/Quote", quoteRequest);
        if (!apiResponse.IsSuccessStatusCode) {
            var errorContent = await apiResponse.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Bank {{BankName}} zwrócił błąd ({apiResponse.StatusCode}) przy POST na quote: {errorContent}");
        }

        var response = await apiResponse.Content.ReadFromJsonAsync<LecturerBankPostQuoteResponse>();
        if (response == null) {
            throw new Exception($"Bank {Name} zwrócił błąd przy POST na quote");
        }

        quoteDto.BankName = Name;
        quoteDto.Id = response.QuoteId;
        quoteDto.InstallmentAmount = response.InstalmentAmount;
        quoteDto.CreatedDate = response.CreatedDate;

        return quoteDto;
    }

    public async Task<OfferDto> PostOfferAsync(OfferDto offerDto) {
        var tokenDto = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto);

        var requestData = new {
            offerDto.QuoteDto.Id,
            offerDto.PersonalDataModel,
            offerDto.PersonalDocumentModel,
            offerDto.JobDetails
        };


        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var apiResponse = await _client.PostAsJsonAsync("v1/Offer",requestData, options);

        var result = await apiResponse.Content.ReadAsStringAsync();
        if (!apiResponse.IsSuccessStatusCode) {
            throw new HttpRequestException($"Bank rzucił błąd: {result}");
        }
        var jsonSettings = new JsonSerializerOptions() {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<PostOfferResponse>(result, jsonSettings);

        if (response == null) {
            throw new Exception($"Bank {Name} zwrócił błąd przy POST na offer");
        }

        offerDto.BankName = Name;
        offerDto.OfferId = response.OfferId.ToString();
        offerDto.CreatedAt = response.CreateDate;
        offerDto.QuoteDto.InstallmentAmount = response.InstalementAmount;

        return offerDto;
    }

    public async Task<OfferDto?> GetOfferByIdAsync(int externalOfferId) {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var apiResponse = await _client.GetAsync($"v1/Offer/{externalOfferId}");
        if (!apiResponse.IsSuccessStatusCode) {
            return null;
        }
        var response = await apiResponse.Content.ReadFromJsonAsync<GetOfferByIdResponse>();
        if (response == null) {
            return null;
        }

        return new OfferDto {
            OfferId = response.Id.ToString(),
            BankName = Name,
            QuoteDto = new QuoteDto {
                Id = response.InquireId ?? 0,
                RequestedAmount = response.RequestedAmount,
                InstalmentNumber = response.RequestedPeriodInMonth,
                InstallmentAmount = response.MonthlyInstallment
            },
            CreatedAt = DateTime.TryParse(response.CreateDate, out var createdDate) ? createdDate : null,
            UpdatedAt = DateTime.TryParse(response.UpdateDate, out var updatedDate) ? updatedDate : null,
            StatusDescription = response.StatusDescription,
            ApprovedBy = response.ApprovedBy,
            Percentage = response.Percentage,
            DocumentLink = response.DocumentLink,
            DocumentLinkValidDate = DateTime.TryParse(response.DocumentLinkValidDate, out var date) ? date : null
        };
    }

    public async Task<bool> PostDocument(int externalOfferId, DocumentDto documentDto) {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var version = "1"; 
        var key = documentDto.DocumentKey ?? Guid.NewGuid().ToString();

        var uploadUrl = $"v{version}/Offer/{externalOfferId}/document/{key}/upload";

        using var content = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(documentDto.Content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(documentDto.ContentType ?? "application/pdf");

        content.Add(fileContent, "formFile", documentDto.FileName);

        var apiResponse = await _client.PostAsync(uploadUrl, content);

        if (apiResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized || apiResponse.StatusCode == System.Net.HttpStatusCode.Forbidden) {
            return false;
        }

        return apiResponse.IsSuccessStatusCode;
    }

    private record LecturerBankPostQuoteResponse(int QuoteId, MoneyModel InstalmentAmount, DateTime CreatedDate);

    private class PostOfferResponse {
        public int InternalId { get; set; }
        [JsonPropertyName("offerId")]
        public int OfferId { get; set; }
        [JsonPropertyName("instalmentAmount")]
        public required MoneyModel InstalementAmount { get; set; }
        [JsonPropertyName("createDate")]
        public DateTime CreateDate { get; set; }
    }

    private class GetOfferByIdResponse {
        public int Id { get; set; }
        public double Percentage { get; set; }
        public required MoneyModel MonthlyInstallment { get; set; }
        public required MoneyModel RequestedAmount { get; set; }
        public int RequestedPeriodInMonth { get; set; }
        public int? StatusId { get; set; }
        public string? StatusDescription { get; set; }
        public int? InquireId { get; set; }
        public string? CreateDate { get; set; }
        public string? UpdateDate { get; set; }
        public string? ApprovedBy { get; set; }
        public string? DocumentLink { get; set; }
        public string? DocumentLinkValidDate { get; set; }
    }
}

