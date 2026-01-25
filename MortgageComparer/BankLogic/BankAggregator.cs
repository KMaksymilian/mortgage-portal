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

    public async Task<OfferDto> PostOfferFromSpecificBankAsync(OfferDto offer, string targetBankName) {
        var provider = _bankProviders.FirstOrDefault(b => b.Name.Equals(targetBankName, StringComparison.OrdinalIgnoreCase));

        if (provider == null) {
            throw new ArgumentException($"Dostawca dla banku '{targetBankName}' nie  znaleziony.");
        }

        if (!offer.BankName.Equals(targetBankName, StringComparison.OrdinalIgnoreCase)) {
            throw new ArgumentException("Nazwa banku w ofercie nie zgadza sie z docelowym bankiem.");
        }

        return await provider.PostOfferAsync(offer);
    }

    public async Task<OfferDto?> GetOfferByIdFromSpecificBankAsync(string bankName, int offerId) {
        var bankProvider = _bankProviders.FirstOrDefault(b => b.Name == bankName)
            ?? throw new KeyNotFoundException($"Bank provider '{bankName}' not found.");

        return await bankProvider.GetOfferByIdAsync(offerId);
    }
}