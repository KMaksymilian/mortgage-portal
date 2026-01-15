using MortgageComparer.Entities;
using MortgageComparerAPI.Models;

namespace MortgageComparerAPI.Services;

public interface IApiOfferService
{
    public Task<PostOfferResponse> PostOfferAsync(PostOfferRequest request);
    /*public Task<ApiOfferEntity> GetOfferByIdAsync(int offerId);
    public Task<string> GetContractAsync(ContractOfferRequest request);
    public Task PostContractAsync(ContractOfferRequest  request);
    public Task CompleteProccess(GetOfferRequest request);*/
}