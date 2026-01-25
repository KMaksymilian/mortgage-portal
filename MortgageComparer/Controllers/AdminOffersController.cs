using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Data;
using MortgageComparer.DataTransferObjects;
using MortgageComparer.StatesMachine;

namespace MortgageComparer.Controllers;

[ApiController]
[Route("api/admin/offers")]
[Authorize(Roles = "Admin")]
public class AdminOffersController : ControllerBase 
{
    private readonly AppDbContext _context;

    public AdminOffersController(AppDbContext context) 
    {
        _context = context;
    }

    [HttpGet("pending")]
    public async Task<ActionResult<List<OfferDto>>> GetPendingAsync() 
    {
        var offers = await _context.Offers
            .Include(o => o.Quote)
            .Where(o => o.Status == OfferStatus.Pending)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Ok(offers.Select(o => new OfferDto(o)).ToList());
    }

    [HttpPost("{offerId:int}/approve")]
    public async Task<IActionResult> ApproveAsync(int offerId) {
        var offer = await _context.Offers.FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer == null) 
        {
            return NotFound();
        }

        try 
        {
            var sm = new OfferStateMachine(offer);
            sm.Approve();
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    public class RejectBody { public string? Reason { get; set; } }

    [HttpPost("{offerId:int}/reject")]
    public async Task<IActionResult> RejectAsync(int offerId, [FromBody] RejectBody body) 
    {
        var offer = await _context.Offers.FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer == null) 
        {
            return NotFound();
        }

        try 
        {
            var sm = new OfferStateMachine(offer);
            sm.Reject(body?.Reason ?? "Rejected by admin");
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }
}
