using System.Net.Http.Headers;
using System.Text.Json;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparerAPI.Models;

namespace MortgageComparer.BankProviders.Banks;

public class OurBank : IBank
{
    private HttpClient _client;
    public string Name { get; } =  "OurBank";

    public OurBank(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("OurBankApi");
    }
    public async Task<PostQuoteResponse> PostQuoteAsync(PostQuoteRequest request)
    {
        var apiResponse = await _client.PostAsJsonAsync("Quote/x/quote",request);
        if (!apiResponse.IsSuccessStatusCode)
        {
            var errorContent = await apiResponse.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Bank {this.Name} zwrócił błąd ({apiResponse.StatusCode}) przy POST na quote: {errorContent}");
        }

        var response = await apiResponse.Content.ReadFromJsonAsync<QuoteResponse>();
        if (response == null)
        {
            throw new Exception($"Bank {Name} zwrócił błąd przy POST na quote");
        }

        return new PostQuoteResponse()
        {
            BankName = Name,
            ExternalBankQuoteId = response.QuoteId,
            InstalmentAmount = new MoneyModel(response.TotalAmountToPay.Amount, response.TotalAmountToPay.Currency),
            CreatedDate = DateTime.UtcNow
        };
    }

    public async Task<PostOfferResponse> PostOfferAsync(int quoteId, UserEntity user)
    {
        string tokenDto = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto);
        PersonalDataModel personalData = new PersonalDataModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.DateOfBirth?.ToString("yyyy-MM-dd")
        };
        // To do: Zmienić te zmockowane dane
        PersonalDocumentModel governmentDocument = new PersonalDocumentModel
        {
            TypeId = 1,
            Number = "fsfasf"
        };
        JobDetailsModel jobDetails = new JobDetailsModel
        {
            JobTypeId = 1,
            StartDate = user.JobStartDate,
            EndDate = user.JobEndDate,
            Income = new MoneyModel(user.Income.Value, user.IncomeCurrCode)
        };
        PostOfferRequest data = new PostOfferRequest()
        {
            QuoteId = quoteId,
            PersonalData = personalData,
            GovernmentDocument = governmentDocument,
            JobDetails = jobDetails
        };
        
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var apiResponse = await _client.PostAsJsonAsync($"Offer/x/quote/{quoteId}",
            data, options);

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
        response.BankName = Name;
        try 
        {
            var details = await GetOfferDetailsInternalAsync(response.OfferId, tokenDto);
            if(details != null)
            {
                response.QuoteId = quoteId;
                response.DocumentLink = details.DocumentLink;
                response.DocumentLinkValidDate = details.DocumentLinkValidDate;
                response.Percentage = details.Percentage;
                response.InstalementAmount = details.MonthlyInstallment;
            }
        }
        catch(Exception ex)
        {
            throw new Exception($"Nie udało się pobrać szczegółów: {ex.Message}");
        }
        return response;
    }
    private async Task<GetOfferByIdResponse?> GetOfferDetailsInternalAsync(int externalOfferId, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var apiResponse = await _client.GetAsync($"Offer/x/{externalOfferId}");
        if (!apiResponse.IsSuccessStatusCode)
        {
            return null;
        }
        
        var res = await apiResponse.Content.ReadFromJsonAsync<ApiOfferEntity>();
        return new GetOfferByIdResponse()
        {
            Id = res.Id,
            Percentage = res.Percentage,
            MonthlyInstallment = new MoneyModel(new decimal(res.MonthlyInstallementAmount), res.MonthlyInstallementCurrency),
            RequestedAmount = new MoneyModel(res.RequestedAmount, res.RequestedCurrency),
            RequestedPeriodInMonth = res.RequestedPeriodInMonths,
            CreateDate = res.CreatedAt.ToString("yyyy-MM-dd"),
            UpdateDate = res.CreatedAt.ToString("yyyy-MM-dd"),
            DocumentLink = res.DocumentKey,
        };
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
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        return token?.AccessToken;
    }
    private record OurBankPostQuoteResponse(int QuoteId, MoneyModel InstalmentAmount, DateTime CreatedDate);

    public async Task<GetOfferByIdResponse?> GetOfferDetailsByIdAsync(int externalOfferId)
    {
        var res = await _client.GetAsync($"{externalOfferId}");
        if (!res.IsSuccessStatusCode)
        {
            throw new Exception($"Błąd z Get banku {this.Name}");
        }
        return await res.Content.ReadFromJsonAsync<GetOfferByIdResponse>();
    }
    /*
    public async Task<byte[]> GetDocumentByDocumentKeyAsync()
    {
        
    }

    public async Task UploadContractAsync(ContractDataDto contract)
    {
        
    }*/
}