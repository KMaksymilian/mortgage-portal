using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Data;
using MortgageComparer.Models;

namespace MortgageComparer.Controllers;

public class ExternalApiController : ControllerBase
{
    private AppDbContext _context;
    private IConfiguration _configuration;
    private int QuoteId { get; set; }

    public ExternalApiController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;

        _configuration = configuration;
    }


    [HttpPost("Quote")]
    public async Task<IActionResult> GetExternalApiOfferAsync([FromBody] CalculatorRequestModel offer)
    {
        if (offer == null || offer.RequestedAmount == null)
        {
            return BadRequest("Invalid data");
        }

        var tokenUrl = "https://indentitymanager.snet.com.pl/connect/token";
        var clientId = _configuration["ExternalApi:Login"];
        var clientSecret = _configuration["ExternalApi:Secret"];


        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "scope", "MiNI.LoanBank.API" }
        };
        var content = new FormUrlEncodedContent(requestData);
        using var client = new HttpClient();
        var tokenResponse = await client.PostAsync(tokenUrl, content);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var errorContent = await tokenResponse.Content.ReadAsStringAsync();
            return StatusCode(500, errorContent);
        }

        var tokenDto = await tokenResponse.Content.ReadFromJsonAsync<TokenResponseDto>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);

        var apiResponse = await client.PostAsJsonAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Quote", offer);

        var result = await apiResponse.Content.ReadAsStringAsync();
        return Content(result, "application/json");
    }
}