using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> PostOfferAsync([FromBody] CalculatorRequestModel offer)
    {
        if (offer == null)
        {
            return BadRequest("Invalid data");
        }
        
        OfferSummaryDto newOffer = await _offerService.ProcessLoanApplicationAsync(offer);
        
        return Ok(newOffer);
    }
    
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptOfferAsync([FromBody] int quoteId)
    {
        FileResultDto fileResult;
        try
        {
            fileResult = await _offerService.AcceptOfferAsync(quoteId);
            return File(fileResult.Content, fileResult.FileName, fileResult.ContentType);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}


