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
                var res = await matchingBank.PostOfferAsync(quote.ExternalBankQuoteId, user);
                result.Add(res);
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

    public async Task<ContractDataDto> AcceptOfferAsync(OfferEntity offer)
    {
        var bankProvider = _bankProviders.FirstOrDefault(b => b.Name == offer.BankName);
        var res = await bankProvider.GetDocumentByDocumentKeyAsync(int.Parse(offer.ExternalBankOfferId), offer.DocumentLink);
        return res;
    }

    public async Task CompleteOfferAsync(IFormFile file, string bankName, string key, int offerId)
    {
        var bankProvider = _bankProviders.FirstOrDefault(b => b.Name == bankName);
        await bankProvider.PostContractAsync(file, offerId, key);
    }
}