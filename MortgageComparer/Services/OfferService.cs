using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.BankLogic;
using MortgageComparer.Controllers;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.StatesMachine;
using MortgageComparerAPI.Models;

namespace MortgageComparer.Services;

public class OfferService : IOfferService
{
    private readonly BankAggregator _banks;
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IExternalApiService _externalApiService;
    private readonly IUserService _userService;
    public OfferService(AppDbContext context,  IHttpClientFactory httpClientFactory,
        IExternalApiService externalApiService,  IUserService userContextService,
        BankAggregator banks)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _externalApiService = externalApiService;
        _userService = userContextService;
        _banks = banks;
    }
    
    public async Task<List<OfferSummaryDto>> ProcessLoanApplicationAsync(PostQuoteRequest model)
    {
        var userId = _userService.GetUserId();
        if (userId == null)
        {
            throw new DataException("Użytkownik nie istnieje w bazie");
        }
        
        var user = await GetUserByIdAsync(userId.Value);
        if (user == null)
        {
            throw new Exception("Użytkownik niekompletny lub nie istnieje");
        }
        
        var quotes = await _banks.PostQuotesFromAllBanksAsync(model);
        
        var offerEntities = await CreateOfferEntitiesAsync(quotes, user, model);
        List<OfferSummaryDto> offerSummaries = new List<OfferSummaryDto>();

        foreach (var offer in offerEntities)
        {
            offer.UserId = userId.Value;
            _context.Offers.Add(offer);   
        }
        await _context.SaveChangesAsync();
        foreach (var offer in offerEntities)
        {
            offerSummaries.Add(new OfferSummaryDto()
            {
                BankName = offer.BankName,
                InternalId = offer.Id,
                Amount = offer.RequestedMoney?.Amount ?? 0,
                MonthlyInstallment = offer.MonthlyInstallment?.Amount ?? 0,
                Currency = offer.MonthlyInstallment?.CurrencyCode ?? "PLN",
                Percentage = offer.BankPercentage ?? 0
            });
        }

        return offerSummaries;
    }
    private async Task<List<OfferEntity>> CreateOfferEntitiesAsync(IEnumerable<PostQuoteResponse> quotes, 
        UserEntity user, PostQuoteRequest originalRequest)
    {
        var bankResponses = await _banks.PostOfferFromAllBanksAsync(quotes, user);
        
        var entities = new List<OfferEntity>();

        foreach (var response in bankResponses)
        {
            decimal amountVal = originalRequest.RequestedAmount?.Amount ?? 0;
            string currencyVal = originalRequest.RequestedAmount?.CurrencyCode ?? "PLN";
            var freshRequestedMoney = new MoneyDto(amountVal, currencyVal);

            var freshInstallment = new MoneyDto(response.InstalementAmount?.Amount ?? 0,
                response.InstalementAmount?.CurrencyCode ?? "PLN");
            var entity = new OfferEntity
            {
                QuoteId = response.QuoteId,
                BankName = response.BankName,
                ExternalBankOfferId = response.OfferId.ToString(),
                MonthlyInstallment = response.InstalementAmount,
                CreateDate = response.CreateDate,
                RequestedMoney = freshRequestedMoney,
                DocumentLink = response.DocumentLink,
                BankPercentage = response.Percentage
            };

            if (response.DocumentLinkValidDate != null)
            {
                entity.ContractLinkValidDate = DateTime.Parse(response.DocumentLinkValidDate).ToUniversalTime();
            }

            entities.Add(entity);
        }
        
        return entities;
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
    
    private async Task<IEnumerable<PostQuoteResponse>> QuoteExternalApiOfferAsync(PostQuoteRequest offer, int? userId)
    {
        /*var client =  _httpClientFactory.CreateClient();
        
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
        var response = JsonSerializer.Deserialize<PostQuoteResponse>(result, jsonSettings);*/
        var response = await _banks.PostQuotesFromAllBanksAsync(offer);
        return response;
    }

    /*private async Task<List<OfferEntity>> OfferPostToExternalApiAsync(IEnumerable<PostQuoteResponse> quoteResponses, UserEntity user,
                 PostQuoteRequest quoteRequest)
    {
        
        /*var client = _httpClientFactory.CreateClient();
        int quoteId = quoteResponse.QuoteId;

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
            Income = new MoneyDto(user.Income, user.IncomeCurrCode)
        };
        PostOfferRequest data = new PostOfferRequest()
        {
            QuoteId = quoteId,
            PersonalData = personalData,
            GovernmentDocument = governmentDocument,
            JobDetails = jobDetails
        };
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto);
        
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var apiResponse = await client.PostAsJsonAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Offer",
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
        var response = JsonSerializer.Deserialize<PostOfferResponse>(result, jsonSettings);#1#
        List<OfferEntity> offerResponses = new List<OfferEntity>();
        var postOfferResponse = await _banks.PostOfferFromAllBanksAsync(quoteResponses, user);
        foreach (var response in postOfferResponse)
        {
                OfferEntity newOffer = new OfferEntity()
                {
                    BankName = response.BankName,
                    ExternalBankOfferId = response.OfferId.ToString(),
                    MonthlyInstallment = response.InstalementAmount,
                    CreateDate = response.CreateDate,
                    RequestedMoney = quoteRequest.RequestedAmount,
                };
        
                var getResponse = await GetOfferDetailsAsync(newOffer);
        
                if (getResponse == null || getResponse.DocumentLink == null || getResponse.DocumentLinkValidDate == null)
                {
                    throw new Exception("Problem z zewnętrznym api");
                }
        
                newOffer.DocumentLink = getResponse.DocumentLink;
                var documentValidDate = DateTime.Parse(getResponse.DocumentLinkValidDate).ToUniversalTime();

                newOffer.ContractLinkValidDate = documentValidDate;
                newOffer.BankPercentage = getResponse.Percentage;
                
                offerResponses.Add(newOffer);
        }
        
        return offerResponses;
    }*/
    private async Task<List<GetOfferByIdResponse?>> GetOfferDetailsAsync(OfferEntity offer)
    {
        var getOfferByIdResponse = await _banks.GetOfferByIdFromAllBanksAsync(offer);

        return getOfferByIdResponse;
    }

    public async Task<ContractDataDto> AcceptOfferAsync(int quoteId)
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

        ContractDataDto contractData = new ContractDataDto
        {
            Content = result,
            FileName = "text/plain",
            ContentType = $"Umowa_Oferta_{offer.ExternalBankOfferId}.txt"
        };
        await UpdateDatabaseAsync(offer);
        
        return contractData;
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

public class OfferSummaryDto
{
    public string BankName { get; set; }
    public int InternalId { get; set; }
    public decimal Amount { get; set; }
    public decimal MonthlyInstallment { get; set; }
    public int Months;
    public string Currency { get; set; }
    public double Percentage { get; set; }
}