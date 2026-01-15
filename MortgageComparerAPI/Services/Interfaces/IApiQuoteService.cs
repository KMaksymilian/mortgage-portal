using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparerAPI.Models;

namespace MortgageComparerAPI.Services;

public interface IApiQuoteService
{
    public Task<QuoteResponse> ApiPostQuote(QuoteRequest quoteRequest);
    public Task<Quote> GetQuoteByIdAsync(int quoteId);
    public Task<ApiOfferEntity?> GetOfferByQuoteIdAsync(int quoteId);
}