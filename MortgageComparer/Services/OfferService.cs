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
using MortgageComparer.DataTransferObjects;
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
    var bankResponses = await _banks.PostQuotesFromAllBanksAsync(model);
    
    var quoteMap = new Dictionary<int, QuoteEntity>();
    
    foreach (var response in bankResponses)
    {
        int bankId = response.ExternalBankQuoteId; 

        var newQuote = new QuoteEntity
        {
            BankName = response.BankName,
            CreatedAt = DateTime.UtcNow,
            
            ExternalQuoteId = bankId,
            
            InstalmentAmount = response.InstalmentAmount,
            RequestedAmount = model.RequestedAmount.Amount,
            Months = model.InstalmentNumber
        };

        _context.Quotes.Add(newQuote);

        if (!quoteMap.ContainsKey(bankId))
        {
            quoteMap[bankId] = newQuote;
        }
    }

    await _context.SaveChangesAsync();

    var offersToSave = new List<OfferEntity>();
    var offerSummaries = new List<OfferSummaryDto>();

    var rawOfferDetails = await _banks.PostOfferFromAllBanksAsync(bankResponses, user);

    foreach (var detail in rawOfferDetails)
    {
        int linkKey = detail.QuoteId; 
        
        if (quoteMap.ContainsKey(linkKey))
        {
            var parentQuote = quoteMap[linkKey];

            var newOffer = new OfferEntity
            {
                UserId = userId.Value,
                QuoteId = parentQuote.Id,
                
                BankName = detail.BankName,
                ExternalBankOfferId = detail.OfferId.ToString(),
                RequestedMoney = new MoneyDto(model.RequestedAmount.Amount, "PLN"),
                MonthlyInstallment = detail.InstalementAmount,
                BankPercentage = detail.Percentage,
                DocumentLink = detail.DocumentLink,
                CreatedAt = DateTime.UtcNow,
                Status = OfferStatus.Pending
            };
            
            if (!string.IsNullOrEmpty(detail.DocumentLinkValidDate) && 
                DateTime.TryParse(detail.DocumentLinkValidDate, out var validDate))
            {
                newOffer.ContractLinkValidDate = validDate.ToUniversalTime();
            }

            _context.Offers.Add(newOffer);
            offersToSave.Add(newOffer);
        }
    }
    await _context.SaveChangesAsync();
    foreach (var offer in offersToSave)
    {
        offerSummaries.Add(new OfferSummaryDto
        {
            InternalId = offer.Id,
            BankName = offer.BankName,
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
                BankName = response.BankName,
                ExternalBankOfferId = response.OfferId.ToString(),
                MonthlyInstallment = response.InstalementAmount,
                CreatedAt = response.CreateDate,
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
                    && (o.Status == OfferStatus.Approved || o.Status == OfferStatus.Completed))
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OfferDto
                {
                Id = o.Id, 
                LoanAmount = o.RequestedMoney.Amount, 
                MonthlyInstallment = o.MonthlyInstallment != null ?  o.MonthlyInstallment.Amount : 0,
                Currencycode  = o.RequestedMoney.CurrencyCode, 
                IsContractSigned = o.ContractData != null, 
                CreateDate = o.CreatedAt,
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
        var response = await _banks.PostQuotesFromAllBanksAsync(offer);
        return response;
    }
    
    private async Task<List<GetOfferByIdResponse?>> GetOfferDetailsAsync(OfferEntity offer)
    {
        var getOfferByIdResponse = await _banks.GetOfferByIdFromAllBanksAsync(offer);

        return getOfferByIdResponse;
    }

    public async Task<ContractDataDto> AcceptOfferAsync(int offerId)
    {
        var userId = _userService.GetUserId();
        if (userId == null)
        {
            throw new Exception("Nie zalogowany użytkownik");
        }
        
        var offer =  await _context.Offers.FirstOrDefaultAsync(o => o.Id == offerId && o.UserId == userId);
        if (offer == null)
        {
            throw new Exception("Brak takiej oferty w bazie");
        }
        
        var res = await _banks.AcceptOfferAsync(offer);
        
        return res;
    }

    public async Task<List<ApiOfferEntity>?> GetAllOurBankOffersAsync()
    {
        return await _context.OurApiOffers.ToListAsync();
    }


    private async Task UpdateDatabaseAsync(OfferEntity offer)
    {
        _context.Offers.Update(offer);
        await _context.SaveChangesAsync();
    }

    public async Task CompleteOfferAsync(IFormFile file, int offerId)
    {
        var offer = await _context.Offers.FindAsync(offerId);
        if (offer == null)
        {
            throw new Exception("Brak takiej oferty w bazie");
        }
        if (file != null && file.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                offer.SignedContractData = memoryStream.ToArray();
            }
        }

        offer.SignedFileName = $"Podpisana_umowa_{offer.ExternalBankOfferId}.txt";
        string key = offer.DocumentLink.Split("document/").Last();
        await _banks.CompleteOfferAsync(file, offer.BankName, key, int.Parse(offer.ExternalBankOfferId));
        await _context.SaveChangesAsync();
    }
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