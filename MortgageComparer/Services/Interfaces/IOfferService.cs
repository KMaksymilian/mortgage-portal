using MortgageComparer.DataTransferObjects;
using MortgageComparer.Entities;
using MortgageComparer.Models;

namespace MortgageComparer.Services.Interfaces;

public interface IOfferService
{
    public Task<List<OfferSummaryDto>> ProcessLoanApplicationAsync(PostQuoteRequest model);
    public Task<List<OfferDto>> OffersFromDatabaseAsync();
    public Task<ContractDataDto> AcceptOfferAsync(int  quoteId);
    public Task CompleteOfferAsync(IFormFile file, int offerId);
}