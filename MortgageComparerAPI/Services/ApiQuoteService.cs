using Microsoft.EntityFrameworkCore;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparerAPI.Models;
using Models_Money = MortgageComparer.Models.Money;

namespace MortgageComparerAPI.Services;

public class ApiQuoteService : IApiQuoteService
{
    private AppDbContext _context;

    public ApiQuoteService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<QuoteResponse> ApiPostQuote(QuoteRequest quoteRequest)
    {
        Quote quoteToSave = new Quote()
        {
            RequestedAmount = quoteRequest.Amount,
            AmountToPay = quoteRequest.Amount + 100,
            Installments = quoteRequest.InstallmentsCount,
            CreatedAt = DateTime.UtcNow
        };

        await _context.AddAsync(quoteToSave);
        await _context.SaveChangesAsync();

        QuoteResponse quoteResponse = new QuoteResponse()
        {
            QuoteId = quoteToSave.Id,
            TotalAmountToPay = new Money()
            {
                Amount = quoteToSave.AmountToPay,
                Currency = quoteToSave.Currency
            }
        };
        return quoteResponse;
    }

    public async Task<Quote> GetQuoteByIdAsync(int quoteId)
    {
        var res = await _context.OurApiQuotes.FirstOrDefaultAsync(q => quoteId == q.Id);
        return res;
    }

    public async Task<ApiOfferEntity?> GetOfferByQuoteIdAsync(int quoteId)
    {
        var res = await _context.OurApiOffers.FirstOrDefaultAsync(o => o.QuoteId == quoteId);
        return res;
    }
}