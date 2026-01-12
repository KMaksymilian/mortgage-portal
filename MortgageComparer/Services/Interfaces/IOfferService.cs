using MortgageComparer.Entities;
using MortgageComparer.Models;

namespace MortgageComparer.Services.Interfaces;

public interface IOfferService
{
    public Task<OfferSummaryDto> ProcessLoanApplicationAsync(CalculatorRequestModel model);
    public Task<List<OfferDto>> OffersFromDatabaseAsync();
    public Task<FileResultDto> AcceptOfferAsync(int  quoteId);
}