using MortgageComparer.BankProviders;
using MortgageComparer.DataTransferObjects;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparerAPI.Models;

namespace MortgageComparer.BankLogic;

public class BankAggregator
{
    private readonly IEnumerable<IBankService> _bankProviders;

    public BankAggregator(IEnumerable<IBankService> bankProviders)
    {
        _bankProviders = bankProviders;
    }

    public async Task<IEnumerable<QuoteDto>> PostQuotesFromAllBanksAsync(QuoteDto request) =>
    (await Task.WhenAll(_bankProviders.Select(b => b.PostQuoteAsync(request)))).Where(res => res != null);

    public async Task<IEnumerable<OfferDto>> PostOfferFromAllBanksAsync(IEnumerable<OfferDto> offers) {
        var tasks = offers
            .Select(offer => new { offer, provider = _bankProviders.FirstOrDefault(b => b.Name == offer.BankName) })
            .Where(x => x.provider != null)
            .Select(x => x.provider!.PostOfferAsync(x.offer));

        return await Task.WhenAll(tasks);
    }

    public async Task<OfferDto?> GetOfferByIdFromSpecificBankAsync(string bankName, int offerId) {
        var bankProvider = _bankProviders.FirstOrDefault(b => b.Name == bankName)
            ?? throw new KeyNotFoundException($"Bank provider '{bankName}' not found.");

        return await bankProvider.GetOfferByIdAsync(offerId);
    }
}