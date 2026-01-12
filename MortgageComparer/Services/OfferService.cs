using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Controllers;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.StatesMachine;

namespace MortgageComparer.Services;

public class OfferService : IOfferService
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IExternalApiService _externalApiService;
    private readonly IUserService _userService;
    public OfferService(AppDbContext context,  IHttpClientFactory httpClientFactory,
        IExternalApiService externalApiService,  IUserService userContextService)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _externalApiService = externalApiService;
        _userService = userContextService;
    }
    
    public async Task<OfferSummaryDto> ProcessLoanApplicationAsync(CalculatorRequestModel model)
    {
        var userId = _userService.GetUserId();
        if (userId == null)
        {
            throw new DataException("Użytkownik nie istnieje w bazie");
        }
        var token = await _externalApiService.GetTokenAsync();
        
        var user = await GetUserByIdAsync(userId.Value);
        if (user == null)
        {
            throw new Exception("Użytkownik niekompletny lub nie istnieje");
        }
        
        var tempOffer = await QuoteExternalApiOfferAsync(model, userId.Value, token);
        
        var finalOffer = await OfferPostToExternalApiAsync(tempOffer, user, token);
        
        _context.Offers.Add(finalOffer);
        await _context.SaveChangesAsync();

        
        return new OfferSummaryDto
        {
            InternalId = finalOffer.Id,
            Amount = finalOffer.RequestedMoney.Amount,
            MonthlyInstallment = finalOffer.MonthlyInstallment.Amount,
            Currency = finalOffer.MonthlyInstallment.CurrencyCode,
            Percentage = finalOffer.BankPercentage.Value
        };
    }
    
    public async Task<List<OfferDto>> OffersFromDatabaseAsync()
    {
        int? userId = _userService.GetUserId();
        if (userId == null)
        {
            throw new Exception("Brak użytkownika w bazie");
        }
        var result = await _context.Offers
            .Where(o => o.UserId == userId && o.ExternalBankOfferId != null 
                    && (o.Status == OfferStatus.ReadyToBeSigned || o.Status == OfferStatus.Completed))
            .OrderByDescending(o => o.CreateDate)
            .Select(o => new OfferDto
                {
                Id = o.Id, 
                LoanAmount = o.RequestedMoney.Amount, 
                MonthlyInstallment = o.MonthlyInstallment != null ?  o.MonthlyInstallment.Amount : 0,
                Currencycode  = o.RequestedMoney.CurrencyCode, 
                IsContractSigned = o.ContractData != null, 
                CreateDate = o.CreateDate,
                Status = o.Status.ToString(),
                })
                .ToListAsync();
        return result;
    }

    private async Task<UserEntity?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.PersonalDocument)
            .Include(u => u.JobType)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
    
    private async Task<OfferEntity> QuoteExternalApiOfferAsync(CalculatorRequestModel offer, int? userId,
        string tokenDto)
    {
        var client =  _httpClientFactory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto);

        var apiResponse = await client.PostAsJsonAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Quote", offer);
        if (!apiResponse.IsSuccessStatusCode)
        {
            throw new Exception("Problem z api");
        }
        var result = await apiResponse.Content.ReadAsStringAsync();
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<ExternalRequestResponse>(result, jsonSettings);
        OfferEntity newOffer = new OfferEntity
        {
            QuoteId = response.QuoteId,
            UserId = (int)userId,
            RequestedMoney = offer.RequestedAmount
        };
        return newOffer;
    }

    private async Task<OfferEntity> OfferPostToExternalApiAsync(OfferEntity newOffer, UserEntity user,
        string? tokenDto)
    {
        var client = _httpClientFactory.CreateClient();
        int quoteId = newOffer.QuoteId;

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
            Income = new MoneyModel(user.Income, user.IncomeCurrCode)
        };
        var data = new
        {
            quoteId = quoteId,
            personalData = personalData,
            governmentDocument = governmentDocument,
            jobDetails = jobDetails
        };
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto);
        
        var apiResponse = await client.PostAsJsonAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Offer", data);

        var result = await apiResponse.Content.ReadAsStringAsync();
        if (!apiResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Bank rzucił błąd: {result}");
        }
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<PostOfferResponseDto>(result, jsonSettings);
        
        newOffer.ExternalBankOfferId = response.OfferId.ToString();
        newOffer.MonthlyInstallment = response.InstalementAmount;
        newOffer.CreateDate = response.CreateDate;
        
        var getResponse = await GetOfferDetailsAsync(user.Id, client, quoteId, newOffer, tokenDto);
        
        if (getResponse == null || getResponse.DocumentLink == null || getResponse.DocumentLinkValidDate == null)
        {
            throw new Exception("Problem z zewnętrznym api");
        }
        
        newOffer.DocumentLink = getResponse.DocumentLink;
        var documentValidDate = DateTime.Parse(getResponse.DocumentLinkValidDate).ToUniversalTime();

        newOffer.ContractLinkValidDate = documentValidDate;
        newOffer.BankPercentage = getResponse.Percentage;
        
        return newOffer;
    }
    private async Task<GetOfferOfferIdResponseDto?> GetOfferDetailsAsync(int? userId, HttpClient client, int quoteId
        , OfferEntity offer, string tokenDto)
    {
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto);
        
        var apiResponse = await client.GetAsync(
            $"https://mini.loanbank.api.snet.com.pl/api/v1/Offer/{offer.ExternalBankOfferId}");
        
        var result = await apiResponse.Content.ReadAsStringAsync();
        if (!apiResponse.IsSuccessStatusCode)
        {
            return null;
        }
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<GetOfferOfferIdResponseDto>(result, jsonSettings);

        return response;
    }

    public async Task<FileResultDto> AcceptOfferAsync(int quoteId)
    {
        var userId = _userService.GetUserId();
        if (userId == null)
        {
            throw new Exception("Nie zalogowany użytkownik");
        }
        
        var offer =  await _context.Offers.FirstOrDefaultAsync(o => o.Id == quoteId && o.UserId == userId);
        if (offer == null)
        {
            throw new Exception("Brak takiej oferty w bazie");
        }
        
        
        var client = _httpClientFactory.CreateClient();
        var tokenDto = await _externalApiService.GetTokenAsync();
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto);


        string documentKey = offer.DocumentLink.Split("document/").Last();
        
        var apiResponse = await client.GetAsync(
            $"https://mini.loanbank.api.snet.com.pl/api/v1/Offer/{offer.ExternalBankOfferId}/document/{documentKey}");
        if (!apiResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Bank rzucił błąd: {apiResponse.StatusCode}");
        }

        byte[] result = await apiResponse.Content.ReadAsByteArrayAsync();
        if (result == null || result.Length == 0)
        {
            throw new Exception("Problem z pobraniem pliku");
        }

        offer.ContractData = result;
        offer.Status = OfferStatus.ReadyToBeSigned;

        FileResultDto fileResult = new FileResultDto
        {
            Content = result,
            FileName = "text/plain",
            ContentType = $"Umowa_Oferta_{offer.ExternalBankOfferId}.txt"
        };
        await UpdateDatabaseAsync(offer);
        
        return fileResult;
    }

    private async Task UpdateDatabaseAsync(OfferEntity offer)
    {
        _context.Offers.Update(offer);
        await _context.SaveChangesAsync();
    }
}

public class OfferDto
{
    public int Id { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal MonthlyInstallment { get; set; }
    public string? Currencycode { get; set; }
    public bool IsContractSigned { get; set; }
    public DateTime CreateDate { get; set; }
    public string Status { get; set; }
}
public class PostOfferResponseDto
{
    public int InternalId { get; set; }
    [JsonPropertyName("offerId")]
    public int OfferId { get; set; }
    [JsonPropertyName("instalmentAmount")]
    public MoneyModel InstalementAmount { get; set; }
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }
}
public class GetOfferOfferIdResponseDto
{
    public int Id { get; set; }
    public double Percentage { get; set; }
    public MoneyModel MonthlyInstallment { get; set; }
    public MoneyModel RequestedAmount { get; set; }
    public int RequestedPeriodInMonth { get; set; }
    public int StatusId { get; set; }
    public string StatusDescription { get; set; }
    public int InquireId  { get; set; }
    public string CreateDate { get; set; }
    public string UpdateDate { get; set; }
    public string? ApprovedBy  { get; set; }
    public string? DocumentLink { get; set; }
    public string? DocumentLinkValidDate { get; set; }
}

public class OfferSummaryDto
{
    public int InternalId { get; set; }
    public decimal Amount { get; set; }
    public decimal MonthlyInstallment { get; set; }
    public string Currency { get; set; }
    public double Percentage { get; set; }
}

public class FileResultDto
{
    public byte[] Content { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
}