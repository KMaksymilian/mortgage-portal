using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.BankProviders.Banks;
using MortgageComparer.Data;
using MortgageComparer.StatesMachine;
using System.Linq;

namespace MortgageComparer.Controllers;

[ApiController]
[Route("api/admin/offers")]
[Authorize(Roles = "Admin")]
public class AdminOffersController : ControllerBase 
{
    private readonly AppDbContext _db;

    public AdminOffersController(AppDbContext db) 
    {
        _db = db;
    }

    public record AdminOfferDto(
        int Id,
        String Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string UserEmail,
        int RequestedAmount,
        string RequestedCurrency,
        int RequestedPeriodInMonths,
        int MonthlyInstallmentAmount,
        string MonthlyInstallmentCurrency
    );

    public record AdminUserDto(
        int Id,
        string Email,
        string? FirstName,
        string? LastName,
        string? BirthDate,
        object? Income,
        int? JobTypeId,
        DateTime? StartDate,
        DateTime? EndDate,
        int? DocTypeId
    );


    [HttpGet]
    public async Task<ActionResult<List<AdminOfferDto>>> GetForAdminAsync() {
        var offers = await _db.Offers
            .Include(o => o.User)
            .Where(o => o.BankName == "OurBank")
            .OrderByDescending(o => o.UpdatedAt)
            .Select(o => new AdminOfferDto(
                o.Id,
                o.Status.ToString(),
                o.CreatedAt,
                o.UpdatedAt,
                o.User.Email,
                (int)o.RequestedMoney.Amount,
                o.RequestedMoney.CurrencyCode,
                o.Quote != null ? o.Quote.Months : 0,
                (int)o.MonthlyInstallment.Amount,
                o.MonthlyInstallment.CurrencyCode
            ))
            .ToListAsync();

        return Ok(offers);
    }

    [HttpPost("{offerId:int}/approve")]
    public async Task<IActionResult> ApproveAsync(int offerId) {
        var offer = await _db.Offers
          .Where(o => o.BankName == "OurBank")
          .FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer == null) 
        {
            return NotFound();
        }

        if (offer.Status != OfferStatus.Pending && offer.Status != OfferStatus.ContractSigned) 
        {
            return Conflict($"Cannot approve offer in status {offer.Status}");
        }

        offer.Status = OfferStatus.Approved;
        offer.UpdatedAt = DateTime.UtcNow;
        offer.StatusDescription = null;

        await _db.SaveChangesAsync();
        return Ok();
    }

    public record RejectRequest(string Reason);

    [HttpPost("{offerId:int}/reject")]
    public async Task<IActionResult> RejectAsync(int offerId, [FromBody] RejectRequest req) {
        var offer = await _db.Offers
          .Where(o => o.BankName == "OurBank")
          .FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer == null) 
        {
            return NotFound();
        }

        if (offer.Status != OfferStatus.Pending && offer.Status != OfferStatus.ContractSigned) 
        {
            return Conflict($"Cannot reject offer in status {offer.Status}");
        }

        offer.Status = OfferStatus.Rejected;
        offer.UpdatedAt = DateTime.UtcNow;
        offer.StatusDescription = req?.Reason;

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("{offerId:int}/user")]
    public async Task<ActionResult<AdminUserDto>> GetUserForOfferAsync(int offerId) {
        var offer = await _db.Offers
            .Where(o => o.BankName == "OurBank")
            .Include(o => o.User)
                .ThenInclude(u => u.JobType)
            .Include(o => o.User)
                .ThenInclude(u => u.PersonalDocument)
            .FirstOrDefaultAsync(o => o.Id == offerId);


        if (offer == null) 
        {
            return NotFound("Offer not found");
        }
        if (offer.User == null) 
        {
            return NotFound("User not found");
        }

        return Ok(new {
            email = offer.User.Email,
            firstName = offer.User.FirstName,
            lastName = offer.User.LastName,
            birthDate = offer.User.DateOfBirth?.ToString("yyyy-MM-dd"),
            income = new { amount = offer.User.Income, currencyCode = offer.User.IncomeCurrCode },
            startDate = offer.User.JobStartDate,
            endDate = offer.User.JobEndDate,

            job = offer.User.JobType == null ? null : new {
                name = offer.User.JobType.Name,
                description = offer.User.JobType.Description
            },

            document = offer.User.PersonalDocument == null ? null : new {
                name = offer.User.PersonalDocument.Name,
                description = offer.User.PersonalDocument.Description
            }
        });

    }
}
