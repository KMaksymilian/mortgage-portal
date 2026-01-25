using MortgageComparer.DataTransferObjects;

namespace MortgageComparer.Services.Interfaces;

public interface IQuoteService
{
    public Task<IEnumerable<QuoteDto>> PostQuoteAsync(QuoteDto quoteRequest);
    public Task<QuoteDto?> GetQuoteById(int quoteId);
}   