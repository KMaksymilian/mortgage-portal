using Microsoft.EntityFrameworkCore;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparerAPI.Models;


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
        int amountToPay = quoteRequest.Amount + 100;
        QuoteEntity quoteToSave = new QuoteEntity()
        {
            InstalmentAmount = new MoneyModel(amountToPay, quoteRequest.Currency),
            RequestedAmount = new MoneyModel(quoteRequest.Amount, quoteRequest.Currency),
            CreatedAt = DateTime.UtcNow
        };

        await _context.AddAsync(quoteToSave);
        await _context.SaveChangesAsync();

        QuoteResponse quoteResponse = new QuoteResponse()
        {
            QuoteId = quoteToSave.Id,
            TotalAmountToPay = new Money()
            {
                Amount = quoteToSave.InstalmentAmount.Amount,
                Currency = quoteToSave.InstalmentAmount.CurrencyCode
            }
        };
        return quoteResponse;
    }

    public async Task<QuoteEntity?> GetQuoteByIdAsync(int quoteId)
    {
        var res = await _context.Quotes.FirstOrDefaultAsync(q => quoteId == q.Id);
        return res;
    }

    public async Task<OfferEntity?> GetOfferByQuoteIdAsync(int quoteId)
    {
        var res = await _context.Offers.FirstOrDefaultAsync(o => o.QuoteId == quoteId);
        return res;
    }
}