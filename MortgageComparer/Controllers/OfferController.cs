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
    public async Task<IActionResult> AcceptOfferAsync([FromBody] int offerId)
    {
        try
        {
            var contract = await _offerService.AcceptOfferAsync(offerId);
            var offer = await _context.Offers.FindAsync(offerId);
            if (offer == null)
            {
                return NotFound("Oferta nie znaleziona");
            }

            offer.Status = OfferStatus.Approved;
            offer.ContractData = contract.FileContents;
            offer.FileName = contract.FileName;
            await _context.SaveChangesAsync();
            return Ok();
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
        try
        {
            var contract = await _offerService.AcceptOfferAsync(offerId);
            var offer = await _context.Offers.FindAsync(offerId);
            if (offer == null)
            {
                return NotFound("Oferta nie znaleziona");
            }

            offer.Status = OfferStatus.Approved;
            offer.ContractData = contract.FileContents;
            offer.FileName = contract.FileName;
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("{offerId}/Download")]
    public async Task<IActionResult> DownloadContractAsync(int offerId)
    {
        var res = await _context.Offers.FindAsync(offerId);
        
        if (res == null || res.ContractData == null)
        {
            return NotFound("Nie znaleziono umowy dla tej oferty.");
        }

        return File(res.ContractData, "text/plain", res.FileName ?? "umowa.txt");
    }

    [HttpPost("{offerId}/Sign")]
    [Authorize]
    public async Task<IActionResult> SignContractAsync(int offerId, [FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nie przesłano pliku.");
            }

            if (!file.FileName.EndsWith(".txt"))
            {
                return BadRequest("Dozwolone są tylko pliki tekstowe (.txt).");
            }
            var offer = await _context.Offers.FindAsync(offerId);
            if (offer == null)
            {
                return NotFound("Nie znaleziono oferty.");
            }
            
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
            }
            offer.Status = OfferStatus.Completed;
            await _offerService.CompleteOfferAsync(file, offerId);
        
            await _context.SaveChangesAsync();

            return Ok(new { message = "Umowa została podpisana pomyślnie." });
        }
        catch (Exception ex)
        {
            return BadRequest($"Błąd podczas podpisywania: {ex.Message}");
        }
    }
    
}


