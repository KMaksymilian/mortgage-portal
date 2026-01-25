using MortgageComparer.Entities;
using MortgageComparer.Models;

namespace MortgageComparer.Services.Interfaces;

public interface IQuoteService
{
    public Task<IEnumerable<PostQuoteResponse>> PostQuoteAsync(PublicQuoteRequest quoteRequest);
    public Task<OfferEntity> FinalizeQuoteAsync(int userId, FinalizeQuoteRequest request);
}   