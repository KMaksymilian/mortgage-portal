using System.Text.Json.Serialization;
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

    // Zwracanie danych frontendowi, żeby mógł wypisać
    [Authorize]
    [HttpGet]
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

    /*[Authorize]
    [HttpPost("path")]
    public async Task<IActionResult> PostOfferToExternalApiAsync([FromBody] QuoteDto quote)
    {
        var userId =  _userContextService.GetUserId();
        if (userId == null)
        {
            return Unauthorized("Użytkownnik nie jest zalogowany.");
        }
        UserEntity user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return BadRequest("Brak użytkownika w bazie");
        }
        var quoteId = quote.QuoteId;
        PersonalDataDto personalData = new PersonalDataDto()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.DateOfBirth.ToString()
        };
        
        var data = new
        {
            quoteId = quote.QuoteId,
            personalData = personalData,
            
        }
        using var client = new HttpClient();
        
        var apiResponse = await client.PostAsJsonAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Offer",
            data);
        
    }*/
    public class QuoteDto
    {
        public int QuoteId { get; set; }
    }

    public class PersonalDataDto
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [JsonPropertyName("birthDate")]
        public string BirthDate { get; set; }
    }
}























