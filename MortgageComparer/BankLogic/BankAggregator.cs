using MortgageComparer.BankProviders;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparerAPI.Models;

namespace MortgageComparer.BankLogic;

public class BankAggregator
{
    private readonly IEnumerable<IBank> _bankProviders;

    public BankAggregator(IEnumerable<IBank> bankProviders)
    {
        _bankProviders = bankProviders;
    }

    public async Task<IEnumerable<PostQuoteResponse>> PostQuotesFromAllBanksAsync(PostQuoteRequest request)
    {
        List<PostQuoteResponse> result = new  List<PostQuoteResponse>();
        foreach (var bankProvider in _bankProviders)
        {
            var res = await bankProvider.PostQuoteAsync(request);
            if (res != null)
            {
                result.Add(res);   
            }
        }
        return result;
    }

    public async Task<IEnumerable<PostOfferResponse>> PostOfferFromAllBanksAsync(IEnumerable<PostQuoteResponse> quoteResponses, UserEntity user)
    {
        List<PostOfferResponse> result = new  List<PostOfferResponse>();
        foreach (var quote in quoteResponses)
        {
            var matchingBank = _bankProviders.FirstOrDefault(b => b.Name == quote.BankName);
            if (matchingBank != null)
            {
                try
                {
                    var res = await matchingBank.PostOfferAsync(quote.QuoteId, user);
                    result.Add(res);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd przy składaniu oferty w {matchingBank.Name}: {ex.Message}");
                }
            }
        }
        return result;
    }

    public async Task<List<GetOfferByIdResponse>> GetOfferByIdFromAllBanksAsync(OfferEntity offer)
    {
        bool success = int.TryParse(offer.ExternalBankOfferId, out int externalBankId);
        if (!success)
        {
            throw new ArgumentException("Invalid bank ID");
        }
        List<GetOfferByIdResponse> result = new  List<GetOfferByIdResponse>();
        foreach (var bankProvider in _bankProviders)
        {
            var res = await bankProvider.GetOfferDetailsByIdAsync(externalBankId);
            result.Add(res);
        }
        return result;
    }
}