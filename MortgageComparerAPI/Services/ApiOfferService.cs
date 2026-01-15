using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<PostOfferResponse> PostOfferAsync(PostOfferRequest request)
    {
        int quoteId = request.QuoteId;
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
            Percentage = 15f,
            MonthlyInstallementAmount = (int)Math.Round((double)quote.AmountToPay * 1.1),
            MonthlyInstallementCurrency = quote.Currency,

            RequestedAmount = quote.RequestedAmount,
            RequestedCurrency =  quote.Currency,
            RequestedPeriodInMonths = quote.Installments,
            RequestedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            DocumentKey = "1234"
        };
        await _context.OurApiOffers.AddAsync(offer);
        await _context.SaveChangesAsync();
        return new PostOfferResponse()
        {
            OfferId = offer.Id,
            InstallmentAmount = new Money()
            {
                Amount = offer.MonthlyInstallementAmount,
                Currency = offer.MonthlyInstallementCurrency
            },
            CreateDate = offer.RequestedDate
        };
    }

    /*public async Task<PostOfferResponseDto> GetOfferByIdAsync(ApiOfferEntity offerId)
    {
        
    }

    public Task<string> GetContractAsync(ContractOfferRequest request)
    {
        
    }

    public Task PostContractAsync(ContractOfferRequest request)
    {
        
    }

    public Task CompleteProccess(GetOfferRequest request)
    {
        
    }*/
    
}