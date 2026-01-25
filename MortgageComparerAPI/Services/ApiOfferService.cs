using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services;
using MortgageComparerAPI.Models;

namespace MortgageComparerAPI.Services;

public class ApiOfferService : IApiOfferService
{
    private IApiQuoteService _apiQuoteService;
    private AppDbContext _context;

    public ApiOfferService(IApiQuoteService apiQuoteService,  AppDbContext context)
    {
        _apiQuoteService = apiQuoteService;
        _context = context;
    }
    public async Task<PostOfferResponse> PostOfferAsync(PostOfferRequest request, int userId, int quoteId)
    {
        var user =  await _context.OurApiUsers.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        var quote =  await _apiQuoteService.GetQuoteByIdAsync(quoteId);
        if (quote == null)
        {
            throw new BadHttpRequestException("Invalid Quote Id");
        }
        var offerToQuote = await _apiQuoteService.GetOfferByQuoteIdAsync(quoteId);
        if (offerToQuote != null)
        {
            throw new BadHttpRequestException("Offer already exists");
        }
        OfferEntity offer = new OfferEntity()
        {
            QuoteId = quoteId,
            UserId = userId,
            Percentage = 10f,

            Quote = quote,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            
        };
        await _context.Offers.AddAsync(offer);
        user.UpdateUser(request);
        await _context.SaveChangesAsync();
        return new PostOfferResponse()
        {
            OfferId = offer.Id,
            InstallmentAmount = new Money()
            {
                Amount = quote.InstalmentAmount.Amount,
                Currency = quote.InstalmentAmount.CurrencyCode
            },
            CreateDate = offer.CreatedAt
        };
    }

    public async Task<OfferEntity> GetOfferByIdAsync(int offerId)
    {
        var offer = await _context.Offers.FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer == null)
        {
            throw new BadHttpRequestException("Offer not found");
        }
        return offer;
    }
    
    [Produces("text/plain")]
    [ProducesResponseType(typeof(FileResult), 200)]
    public async Task<CustomFile> GetContractAsync(int offerId, string key)
    {
       throw new NotImplementedException();
    }

    public async Task PostContractAsync(IFormFile file, int offerId, string key)
    {
       throw new NotImplementedException();
    }

}