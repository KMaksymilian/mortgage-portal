using Microsoft.EntityFrameworkCore;
using MortgageComparer.BankLogic;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.StatesMachine;

namespace MortgageComparer.Services;

public class QuoteService : IQuoteService
{
    private readonly BankAggregator _banks;
    private AppDbContext _context;

    public QuoteService(BankAggregator banks,  AppDbContext context)
    {
        _banks = banks;
        _context = context;
    }

    public async Task<IEnumerable<PostQuoteResponse>> PostQuoteAsync(PublicQuoteRequest quoteRequest)
    {
        decimal amountToPay = quoteRequest.Amount - quoteRequest.OwnContribution;

        PostQuoteRequest request = new PostQuoteRequest
        {
            RequestedAmount = new MoneyDto(amountToPay, "PLN"),
            InstalmentNumber = quoteRequest.Months
        };
        var bankResponses = await _banks.PostQuotesFromAllBanksAsync(request);
        if (bankResponses == null)
        {
            throw new Exception("No quotes found");
        }
        var results = new List<PostQuoteResponse>();
        foreach (var response in bankResponses)
        {
            QuoteEntity newQuote = new QuoteEntity()
            {
                BankName = response.BankName,
                QuoteId = response.ExternalBankQuoteId,
                InstalmentAmount = new MoneyDto(response.InstalmentAmount.Amount,
                    response.InstalmentAmount.CurrencyCode),
                ExternalQuoteId = response.ExternalBankQuoteId,
                RequestedAmount = quoteRequest.Amount,
                Months = quoteRequest.Months,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Quotes.Add(newQuote);
            await _context.SaveChangesAsync();
            
            results.Add(new PostQuoteResponse 
            {
                InternalId = newQuote.Id,
                BankName = newQuote.BankName,
                InstalmentAmount = newQuote.InstalmentAmount,
            });
        }

        return results;
    }

    public async Task<OfferEntity> FinalizeQuoteAsync(int userId, FinalizeQuoteRequest request)
    {
        var quote = await _context.Quotes.FindAsync(request.QuoteId);

        if (quote == null)
        {
            throw new Exception("Oferta nie istnieje");
        }
        
        
        var user = await _context.Users
            .Include(u => u.PersonalDocument) 
            .Include(u => u.JobType)
            .FirstOrDefaultAsync(u => u.Id == userId);
        user.JobStartDate = request.JobStartDate.ToUniversalTime();
        user.JobEndDate = request.JobEndDate.Value == null ? DateTime.UtcNow :  request.JobEndDate.Value.ToUniversalTime();
        user.Income = (int)request.Earnings;
        user.DateOfBirth = request.BirthDate.ToUniversalTime();
        await _context.SaveChangesAsync();
        
    
        var quoteResponseForAggregator = new PostQuoteResponse
        {
            BankName = quote.BankName,
            ExternalBankQuoteId = quote.ExternalQuoteId.Value,
        };
        
        var offerResponses = await _banks.PostOfferFromAllBanksAsync(
            new List<PostQuoteResponse> { quoteResponseForAggregator }, 
            user
        );
        var response = offerResponses.FirstOrDefault();
        if (response == null)
        {
            throw new Exception("Bank nie zwrócił oferty.");
        }
        
        var finalOffer = new OfferEntity 
        {
            QuoteId = quote.Id,
            BankName = response.BankName,
            UserId = userId,
            RequestedMoney = new MoneyDto(quote.RequestedAmount, "PLN"),
            ExternalBankOfferId = response.OfferId.ToString(),
            MonthlyInstallment = response.InstalementAmount,
            CreatedAt = response.CreateDate.ToUniversalTime(),
            DocumentLink = response.DocumentLink,
            BankPercentage = response.Percentage,
            Status = OfferStatus.Approved
        };
        _context.Offers.Add(finalOffer);
    
        await _context.SaveChangesAsync();
        return  finalOffer;
    }
}