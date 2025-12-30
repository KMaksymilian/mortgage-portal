using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Services;
using MortgageComparer.Services.Interfaces;

namespace MortgageComparer.Controllers;

[Route("api/{controller}")]
public class OfferController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IUserContextService  _userContextService;

    public OfferController(AppDbContext context, IUserContextService userContextService)
    {
        _context = context;
        _userContextService = userContextService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetOffersAsync()
    {
        int? userId =  _userContextService.GetUserId();
        if (userId == null)
        {
            return Unauthorized("Użytkownnik nie jest zalogowany.");
        }
        var result = await _context.Offers
            .Where(o => o.UserId == userId.Value)
            .OrderByDescending(o => o.CreateDate)
            .Select(o => new
            {
                id = o.Id,
                quoteId = o.QuoteId,
                amount = o.RequestedMoney.Amount, 
                currency = o.RequestedMoney.CurrencyCode,
                status = o.Status.ToString(),
                date = o.CreateDate
            })
            .ToListAsync();
        return Ok(result);
    }
}