using MortgageComparer.DataTransferObjects;
using MortgageComparer.Entities;
using MortgageComparer.Services;
using MortgageComparerAPI.Models;

namespace MortgageComparer.BankProviders;

public interface IBankService
{
    string Name { get; }
    public Task<QuoteDto> PostQuoteAsync(QuoteDto quoteDto);
    public Task<OfferDto> PostOfferAsync(OfferDto offerDto);
    public Task<OfferDto?> GetOfferByIdAsync(int externalOfferId);
    public Task<bool> PostDocument(int externalOfferId, DocumentDto documentDto);
}