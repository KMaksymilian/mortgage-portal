using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.BankLogic;
using MortgageComparer.DataTransferObjects;
using MortgageComparer.Data;
using MortgageComparer.Models;
using MortgageComparer.Services;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.StatesMachine;

namespace MortgageComparer.Controllers;

[Route("api/{controller}")]
public class OfferController : ControllerBase
{
    private readonly IOfferService _offerService;
    private readonly IUserService _userService;
    private AppDbContext  _context;

    public OfferController(IOfferService offerService,  IUserService userService,  AppDbContext context)
    {
        _offerService = offerService;
        _userService = userService;
        _context = context;
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
    [HttpPut("{offerId}/Reject")]
    [Authorize]
    public async Task<IActionResult> RejectOfferAsync(int offerId)
    {
        var userId = _userService.GetUserId();
    
        var offer = await _context.Offers.FindAsync(offerId);

        if (offer == null)
        {
            return NotFound();
        }
        
        offer.Status = OfferStatus.Rejected; 

        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{offerId}/Accept")]
    [Authorize]
    public async Task<IActionResult> AcceptOfferIdAsync(int offerId)
    {
        var userId = _userService.GetUserId();
    
        var offer = await _context.Offers.FindAsync(offerId);

        if (offer == null)
        {
            return NotFound();
        }
        
        offer.Status = OfferStatus.Approved; 

        await _context.SaveChangesAsync();
        return Ok();
    }
    
}


