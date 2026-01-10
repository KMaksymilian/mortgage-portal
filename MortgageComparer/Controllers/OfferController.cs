using System.Net.Http.Headers;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.StatesMachine;

namespace MortgageComparer.Controllers;

[Route("api/{controller}")]
public class OfferController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IUserContextService  _userContextService;
    private readonly IExternalApiService _externalApiService;
    IHttpClientFactory _httpClientFactory;

    public OfferController(AppDbContext context, IUserContextService userContextService,
        IExternalApiService externalApiService,  IConfiguration configuration,  IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _userContextService = userContextService;
        _externalApiService = externalApiService;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetOffersAsync()
    {
        int? userId =  _userContextService.GetUserId();
        if (userId == null)
        {
            return Unauthorized("Użytkownnik nie jest zalogowany.");
        }
        
        var result = await _context.Offers
            .Where(o => o.UserId == userId.Value && o.ExternalBankOfferId != null 
                                                 && o.Status == OfferStatus.ReadyToBeSigned || o.Status == OfferStatus.Completed )
            .OrderByDescending(o => o.CreateDate)
            .Select(o => new
            {
                id = o.Id,
                quoteId = o.QuoteId,
                amount = o.RequestedMoney.Amount, 
                currency = o.RequestedMoney.CurrencyCode,
                isContractSigned = o.ContractData != null,
                date = o.CreateDate,
                contract = o.ContractData
            })
            .ToListAsync();
        return Ok(result);
    }

    [Authorize]
    [HttpPost("Quote")]
    public async Task<IActionResult> PostOfferAsync([FromBody] CalculatorRequestModel offer)
    {
        if (offer == null)
        {
            return BadRequest("Invalid data");
        }
        
        var userId = _userContextService.GetUserId();
        if (userId == null)
        {
            return Unauthorized("Użytkownik nie jest zalogowany.");
        }
        var tokenDto = await _externalApiService.GetTokenAsync();
        
        UserEntity user = await _context.Users
            .Include(u => u.PersonalDocument)
            .Include(u => u.JobType)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return BadRequest("Użytkownik nie konfiguruje w bazie");
        }
        
        var client =  _httpClientFactory.CreateClient();
        OfferEntity newOffer;
        try
        {
            newOffer = await GetExternalApiOfferAsync(offer, userId, client, tokenDto);
        }
        catch (Exception ex)
        {
            return BadRequest("Błąd pobierania danych");
        }
        int quoteId = newOffer.QuoteId;

        PersonalDataModel personalData = new PersonalDataModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.DateOfBirth?.ToString("yyyy-MM-dd")
        };
        PersonalDocumentModel governmentDocument = new PersonalDocumentModel
        {
            TypeId = user.PersonalDocument.PersonalDocumentId,
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
            return BadRequest($"Bank rzucił błąd: {result}");
        }
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<PostOfferResponseDto>(result, jsonSettings);
        
        newOffer.ExternalBankOfferId = response.OfferId.ToString();
        newOffer.MonthlyInstallment = response.InstalementAmount;
        newOffer.CreateDate = response.CreateDate;
        
        var getResponse = await GetOfferDetailsAsync(userId, client, quoteId, newOffer, tokenDto);
        
        if (getResponse == null || getResponse.DocumentLink == null || getResponse.DocumentLinkValidDate == null)
        {
            return BadRequest("Problem z zewnętrznym api");
        }
        
        newOffer.DocumentLink = getResponse.DocumentLink;
        var documentValidDate = DateTime.Parse(getResponse.DocumentLinkValidDate).ToUniversalTime();

        newOffer.ContractLinkValidDate = documentValidDate;
        newOffer.BankPercentage = getResponse.Percentage;

        _context.Offers.Add(newOffer);
        await _context.SaveChangesAsync();
        response.InternalId = newOffer.Id;
        
        var res = new
        {
            internalId = newOffer.Id,
            amount = newOffer.RequestedMoney.Amount,
            monthlyInstallment = newOffer.MonthlyInstallment.Amount,
            currency = newOffer.MonthlyInstallment.CurrencyCode,
            percentage = getResponse.Percentage
        };
        
        return Ok(res);
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
    
    private async Task<OfferEntity> GetExternalApiOfferAsync(CalculatorRequestModel offer, int? userId, HttpClient client
    , string tokenDto)
    {
        
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto);

        var apiResponse = await client.PostAsJsonAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Quote", offer);

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
    
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptOfferAsync([FromBody] int quoteId)
    {
        var userId = _userContextService.GetUserId();
        if (userId == null)
        {
            return Unauthorized("Nie zalogowany użytkownik");
        }
        
        var offer =  await _context.Offers.FirstOrDefaultAsync(o => o.Id == quoteId && o.UserId == userId);
        if (offer == null)
        {
            return NotFound("Brak takiej oferty w bazie");
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
            return BadRequest($"Bank rzucił błąd: {apiResponse.StatusCode}");
        }

        byte[] result = await apiResponse.Content.ReadAsByteArrayAsync();
        if (result == null || result.Length == 0)
        {
            return BadRequest("Problem z pobraniem pliku");
        }

        offer.ContractData = result;
        offer.Status = OfferStatus.ReadyToBeSigned;
        _context.Offers.Update(offer);
        await _context.SaveChangesAsync();
        
        return File(result, "text/plain",$"Umowa do oferty: {offer.ExternalBankOfferId}");
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
        public MoneyModel MonthylInstallment { get; set; }
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
}