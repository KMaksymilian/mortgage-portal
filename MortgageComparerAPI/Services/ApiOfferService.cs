using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Services;
using MortgageComparerAPI.Models;
using Money = MortgageComparer.Models.Money;

namespace MortgageComparerAPI.Services;

public class ApiOfferService : IApiOfferService
{
    private IApiQuoteService _apiQuoteService;
    private AppDbContext _context;

    public ApiOfferService(IApiQuoteService apiQuoteService,  AppDbContext context)
    {
        _apiQuoteService = apiQuoteService;
        _context = context;
    }
    public async Task<PostOfferResponse> PostOfferAsync(PostOfferRequest request, int userId, int quoteId)
    {
        var user =  await _context.OurApiUsers.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        var quote =  await _apiQuoteService.GetQuoteByIdAsync(quoteId);
        if (quote == null)
        {
            throw new BadHttpRequestException("Invalid Quote Id");
        }
        var offerToQuote = await _apiQuoteService.GetOfferByQuoteIdAsync(quoteId);
        if (offerToQuote != null)
        {
            throw new BadHttpRequestException("Offer already exists");
        }
        // To do: Poprawić zmockowane dane
        ApiOfferEntity offer = new ApiOfferEntity()
        {
            QuoteId = quoteId,
            UserId = userId,
            Percentage = 15f,
            MonthlyInstallementAmount = (int)Math.Round((double)quote.AmountToPay * 1.1) / quote.Installments,
            MonthlyInstallementCurrency = quote.Currency,

            RequestedAmount = quote.RequestedAmount,
            RequestedCurrency =  quote.Currency,
            RequestedPeriodInMonths = quote.Installments,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DocumentKey = "1234"
        };
        await _context.OurApiOffers.AddAsync(offer);
        user.UpdateUser(request);
        await _context.SaveChangesAsync();
        return new PostOfferResponse()
        {
            OfferId = offer.Id,
            InstallmentAmount = new Money()
            {
                Amount = offer.MonthlyInstallementAmount,
                Currency = offer.MonthlyInstallementCurrency
            },
            CreateDate = offer.CreatedAt
        };
    }

    public async Task<ApiOfferEntity> GetOfferByIdAsync(int offerId)
    {
        var offer = await _context.OurApiOffers.FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer == null)
        {
            throw new BadHttpRequestException("Offer not found");
        }
        return offer;
    }
    
    [Produces("text/plain")]
    [ProducesResponseType(typeof(FileResult), 200)]
    public async Task<CustomFile> GetContractAsync(int offerId, string key)
    {
        var offer = await _context.OurApiOffers
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer == null)
        {
            throw new BadHttpRequestException("Offer not found");
        }

        if (!(offer.DocumentKey == key))
        {
            throw new BadHttpRequestException("Invalid Document Key");
        }
        
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "ContractTemplate.txt");
        string content = System.IO.File.ReadAllText(templatePath);
        content = content.Replace("{{DaneDoUmowy}}", offer.User.FirstName  + " " + offer.User.LastName);
        var contract = Encoding.UTF8.GetBytes(content);
        offer.Contract = contract;
        await  _context.SaveChangesAsync();
        return new CustomFile()
        {
            FileContents = contract, 
            ContentType = "text/plain",
            FileName = $"Umowa_{offerId}.txt"
        };
    }

    public async Task PostContractAsync(IFormFile file, int offerId, string key)
    {
        var offer = await _context.OurApiOffers.FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer == null)
        {
            throw new Exception($"Nie znaleziono oferty o ID {offerId} w tabeli OurApiOffers.");
        }
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            offer.SignedContract = memoryStream.ToArray();
        
            await _context.SaveChangesAsync();
        }
    }

    /*
    public Task CompleteProccess(GetOfferRequest request)
    {
        
    }*/
}