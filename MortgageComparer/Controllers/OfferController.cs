using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.BankLogic;
using MortgageComparer.Data;
using MortgageComparer.DataTransferObjects;
using MortgageComparer.Models;
using MortgageComparer.Services;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.StateMachine;
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
            offers = await _offerService.GetAllAsync();
            return Ok(offers);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("Create")]
    public async Task<IActionResult> PostOfferAsync([FromBody] OfferDto offer)
    {
        if (offer == null)
        {
            return BadRequest("Invalid data");
        }
        
        OfferDto newOffers = await _offerService.CreateAsync(offer);
        
        return Ok(newOffers);
    }

    [HttpPost("{id}/execute")]
    public async Task<IActionResult> ExecuteActionAsync(int id, [FromBody] ActionRequest request) {
        try {
            var action = OfferActionFactory.Create(request);

            return await _offerService.ExecuteActionAsync(id, action)
                ? Ok(new { Message = $"Action {request.Action} executed successfully." })
                : NotFound($"Offer with ID {id} not found.");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception) {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

}


