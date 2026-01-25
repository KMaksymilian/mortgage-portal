using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Google.Apis.Auth;
using MortgageComparer.BankProviders;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparerAPI.Models;

namespace MortgageComparer.BankLogic.Banks;

public class LecturerBank : IBank
{
    private IConfiguration _configuration;
    private HttpClient _client;
    public string Name { get; } =  "LecturerBank";

    public LecturerBank(IHttpClientFactory clientFactory,  IConfiguration configuration)
    {
        _client = clientFactory.CreateClient("LecturerBankApi");
        _configuration = configuration;
    }
    public async Task<PostQuoteResponse> PostQuoteAsync(PostQuoteRequest request)
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var apiResponse = await _client.PostAsJsonAsync("v1/Quote",request);
        if (!apiResponse.IsSuccessStatusCode)
        {
            var errorContent = await apiResponse.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Bank {{BankName}} zwrócił błąd ({apiResponse.StatusCode}) przy POST na quote: {errorContent}");
        }

        var response = await apiResponse.Content.ReadFromJsonAsync<LecturerBankPostQuoteResponse>();
        if (response == null)
        {
            throw new Exception($"Bank {Name} zwrócił błąd przy POST na quote");
        }

        return new PostQuoteResponse()
        {
            BankName =  Name,
            ExternalBankQuoteId = response.QuoteId,
            InstalmentAmount = response.InstalmentAmount,
            CreatedDate = response.CreatedDate
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
            TypeId = user.PersonalDocument.Id,
            Number = "fsfasf"
        };
        JobDetailsModel jobDetails = new JobDetailsModel
        {
            JobTypeId = (int)user.JobTypeId,
            StartDate = user.JobStartDate,
            EndDate = user.JobEndDate,
            Income = new MoneyDto(user.Income.Value, user.IncomeCurrCode)
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
        var apiResponse = await _client.PostAsJsonAsync("v1/Offer",
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
            var details = await GetOfferDetailsByIdAsync(response.OfferId);
            if(details != null)
            {
                response.QuoteId = quoteId;
                response.DocumentLink = details.DocumentLink;
                response.DocumentLinkValidDate = details.DocumentLinkValidDate;
                response.Percentage = details.Percentage;
            }
        }
        catch(Exception ex)
        {
            throw new Exception($"Nie udało się pobrać szczegółów: {ex.Message}");
        }
        return response;
    }
    public async Task<GetOfferByIdResponse?> GetOfferDetailsByIdAsync(int externalOfferId)
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var apiResponse = await _client.GetAsync($"v1/Offer/{externalOfferId}");
        if (!apiResponse.IsSuccessStatusCode)
        {
            return null;
        }
        
        return await apiResponse.Content.ReadFromJsonAsync<GetOfferByIdResponse>();
    }

    public async Task<GetOfferByIdResponse?> GetOfferByIdAsync(OfferEntity offer)
    {
        var tokenDto = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto);
        
        var apiResponse = await _client.GetAsync(
            $"Offer/{offer.ExternalBankOfferId}");
        
        var result = await apiResponse.Content.ReadAsStringAsync();
        if (!apiResponse.IsSuccessStatusCode)
        {
            return null;
        }
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<GetOfferByIdResponse>(result, jsonSettings);
        return response;
    }

    public async Task<ContractDataDto> GetDocumentByDocumentKeyAsync(int offerId, string key)
    {
        var tokenDto = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto);
        string documentKey = key.Split("document/").Last();
        var res= await _client.GetAsync($"v1/Offer/{offerId}/document/{documentKey}");
        if (!res.IsSuccessStatusCode)
        {
            var errorContent = await res.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Błąd pobierania ({res.StatusCode}): {errorContent}");
        }
        var fileBytes = await res.Content.ReadAsByteArrayAsync();
        
        var contentType = res.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        var fileName = res.Content.Headers.ContentDisposition?.FileNameStar 
                       ?? res.Content.Headers.ContentDisposition?.FileName 
                       ?? "umowa.txt";
        return new ContractDataDto
        {
            FileContents = fileBytes,
            ContentType = contentType,
            FileName = fileName
        };
    }

    public async Task PostContractAsync(IFormFile file, int offerId, string key)
    {
        var tokenDto = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto);
        
        var version = "1"; 
        var url = $"v{version}/Offer/{offerId}/document/{key}upload";

        using (var content = new MultipartFormDataContent())
        {
            using (var stream = file.OpenReadStream())
            {
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        
                content.Add(streamContent, "formFile", file.FileName); 

                var res = await _client.PostAsync(url, content);

                if (!res.IsSuccessStatusCode)
                {
                    var error = await res.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Błąd uploadu ({res.StatusCode}): {error}");
                }

                var completeRes = await _client.PostAsync($"v{version}/Offer/{offerId}/complete", null);
                if (!completeRes.IsSuccessStatusCode)
                {
                    var error = await completeRes.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Błąd KROKU 2 (Complete): {completeRes.StatusCode}, {error}");
                }
            }
        }
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
        var response =  await _client.PostAsync(tokenUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        return token?.AccessToken;
    }

    public Task CompleteOfferAsync(int offerId, string key)
    {
        throw new NotImplementedException();
    }

    private record LecturerBankPostQuoteResponse(int QuoteId, MoneyDto InstalmentAmount, DateTime CreatedDate);
    public record DocumentContent(string Payload, string FileName, string ContentType);
}
