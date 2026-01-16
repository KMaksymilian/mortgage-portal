using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Entities;
using MortgageComparerAPI.Models;

namespace MortgageComparerAPI.Services;

public interface IApiOfferService
{
    public Task<PostOfferResponse> PostOfferAsync(PostOfferRequest request, int userId, int quoteId);
    public Task<ApiOfferEntity> GetOfferByIdAsync(int offerId);
    public Task<CustomFile> GetContractAsync(int offerId, string key);
    /*
    public Task PostContractAsync(ContractOfferRequest  request);
    public Task CompleteProccess(GetOfferRequest request);*/
}