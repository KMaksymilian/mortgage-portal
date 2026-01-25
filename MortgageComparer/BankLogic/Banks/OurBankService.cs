using MortgageComparer.DataTransferObjects;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparerAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MortgageComparer.BankProviders.Banks;

public class OurBank : IBankService {
    private HttpClient _client;
    public string Name { get; } =  "OurBank";

    public OurBank(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("OurBankApi");
    }
    public async Task<QuoteDto> PostQuoteAsync(QuoteDto quoteDto) {


        var request = new {
            RequestedAmount = quoteDto.RequestedAmount,
            InstalmentNumber = quoteDto.InstalmentNumber
        };

        var apiResponse = await _client.PostAsJsonAsync("Quote/x/quote", request);

        if (!apiResponse.IsSuccessStatusCode) {
            var errorContent = await apiResponse.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Bank {Name} zwrócił błąd ({apiResponse.StatusCode}): {errorContent}");
        }

        var response = await apiResponse.Content.ReadFromJsonAsync<QuoteResponse>();
        if (response == null) {
            throw new Exception($"Bank {Name} zwrócił pustą odpowiedź.");
        }

        quoteDto.BankName = Name;
        quoteDto.Id = response.QuoteId;
        quoteDto.InstallmentAmount = response.TotalAmountToPay;
        quoteDto.CreatedDate = DateTime.UtcNow;

        return quoteDto;
    }

    public async Task<OfferDto> PostOfferAsync(OfferDto offerDto)
    {
        var tokenDto = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto);
        
        var data = new {
            offerDto.QuoteDto.Id,
            offerDto.PersonalDataModel,
            offerDto.PersonalDocumentModel, 
            offerDto.JobDetails
        };
        
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var apiResponse = await _client.PostAsJsonAsync($"Offer/x/quote/{offerDto.QuoteDto.Id}",data, options);

        var result = await apiResponse.Content.ReadAsStringAsync();
        if (!apiResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Bank rzucił błąd: {result}");
        }
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<PostOfferResponse>(result, jsonSettings);
        if (response == null)
        {
            throw new Exception($"Bank {Name} zwrócił pustą odpowiedź.");
        }
        offerDto.BankName = Name;
        offerDto.OfferId = response.OfferId;
        offerDto.CreatedAt = response.CreateDate;
        offerDto.QuoteDto.InstallmentAmount = response.InstalementAmount;
        return offerDto;
    }

    public async Task<OfferDto?> GetOfferByIdAsync(int externalOfferId) {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var apiResponse = await _client.GetAsync($"x/{externalOfferId}");
        if (!apiResponse.IsSuccessStatusCode) {
            return null;
        }
        var response = await apiResponse.Content.ReadFromJsonAsync<GetOfferByIdResponse>();
        if (response == null) {
            return null;
        }

        return new OfferDto {
            OfferId = response.Id,
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

        var key = documentDto.DocumentKey ?? Guid.NewGuid().ToString();
        var requestUrl = $"api/x/Offer/{externalOfferId}/document/{key}/upload";

        using var content = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(documentDto.Content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(documentDto.ContentType ?? "application/pdf");

        content.Add(fileContent, "file", documentDto.FileName);

        var response = await _client.PostAsync(requestUrl, content);

        return response.IsSuccessStatusCode;
    }

    private async Task<string?> GetTokenAsync()
    {
        var data = new ApiLoginRequest
        {
            ClientSecret = "12345",
            Email = "faskfjsakfa@"
        };
        var response =  await _client.PostAsJsonAsync("Auth/x", data);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var token = await response.Content.ReadFromJsonAsync<ApiLoginResponse>();
        if (token == null)
        {
            return null;
        }
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        return token?.AccessToken;
    }
 
    private record QuoteResponse(int QuoteId, MoneyModel TotalAmountToPay);

    private class PostOfferResponse {
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