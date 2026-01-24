using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services;
using MortgageComparerAPI.Models;

namespace MortgageComparer.BankProviders;

public interface IBank
{
    string Name { get; }
    public Task<PostQuoteResponse> PostQuoteAsync(PostQuoteRequest request);

    public Task<PostOfferResponse> PostOfferAsync(int quoteId, UserEntity user);
    public Task<GetOfferByIdResponse?> GetOfferDetailsByIdAsync(int externalOfferId);

    /*public Task<GetOfferByIdResponse> GetOfferByIdAsync(OfferEntity offer);*/
    /*
    public Task<byte[]> GetDocumentByDocumentKeyAsync();
    public Task UploadContractAsync(ContractDataDto contract)
    */
}