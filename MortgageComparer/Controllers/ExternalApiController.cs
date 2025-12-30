using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Controllers.Interfaces;
using MortgageComparer.Services.Interfaces;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace MortgageComparer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExternalApiController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IExternalApiService _externalApiService;
    private readonly IUserContextService _userContextService;

    public ExternalApiController(AppDbContext context, IConfiguration configuration,
        IExternalApiService externalApiService, IUserContextService userContextService)
    {
        _context = context;
        _configuration = configuration;
        _externalApiService = externalApiService;
        _userContextService = userContextService;
    }

    [Authorize]
    [HttpPost("Quote")]
    public async Task<IActionResult> GetExternalApiOfferAsync([FromBody] CalculatorRequestModel offer)
    {
        if (offer == null || offer.RequestedAmount == null)
        {
            return BadRequest("Invalid data");
        }

        int? userId = _userContextService.GetUserId();
        if (userId == null)
        {
            return Unauthorized("Użytkownik nie jest zalogowany.");
        }
        
        using var client = new HttpClient();
        var tokenDto = await _externalApiService.GetTokenAsync();
        

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto);

        var apiResponse = await client.PostAsJsonAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Quote", offer);

        var result = await apiResponse.Content.ReadAsStringAsync();
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<ExternalRequestResponse>(result, jsonSettings);
        OfferEntity offerEntity = new OfferEntity
        {
            UserId = (int)userId,
            QuoteId = response.QuoteId,
            RequestedMoney = offer.RequestedAmount
        };
        _context.Offers.Add(offerEntity);
        await _context.SaveChangesAsync();
        return Content(result, "application/json");
    }
}