using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MortgageComparer.BankLogic;
using MortgageComparer.Models;
using MortgageComparer.Services;
using MortgageComparer.Services.Interfaces;

namespace MortgageComparer.Controllers;

[Route("api/{controller}")]
public class OfferController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OfferController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetOffersAsync()
    {
        List<OfferDto> offers;
        try
        {
            offers = await _offerService.OffersFromDatabaseAsync();
            return Ok(offers);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("Quote")]
    public async Task<IActionResult> PostOfferAsync([FromBody] PostQuoteRequest offer)
    {
        if (offer == null)
        {
            return BadRequest("Invalid data");
        }
        
        List<OfferSummaryDto> newOffers = await _offerService.ProcessLoanApplicationAsync(offer);
        
        return Ok(newOffers);
    }
    
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptOfferAsync([FromBody] int quoteId)
    {
        ContractDataDto contractData;
        try
        {
            contractData = await _offerService.AcceptOfferAsync(quoteId);
            return File(contractData.Content, contractData.FileName, contractData.ContentType);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}


