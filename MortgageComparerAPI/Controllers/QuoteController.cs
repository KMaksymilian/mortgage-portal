using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Models;
using MortgageComparerAPI.Models;
using MortgageComparerAPI.Services;

namespace MortgageComparerAPI.Controllers;

[ApiController]
[Route("api/[controller]/x")]
[Authorize]
public class QuoteController : ControllerBase
{
    private readonly IApiQuoteService _apiQuoteService;

    public QuoteController(IApiQuoteService apiQuoteService)
    {
        _apiQuoteService = apiQuoteService;
    }

    [HttpPost("quote")]
    public async Task<IActionResult> QuotePostAsync([FromBody] PostQuoteRequest quoteRequest)
    {
        QuoteRequest quote = new QuoteRequest
        {
            Amount = (int)quoteRequest.RequestedAmount.Amount,
            InstallmentsCount = quoteRequest.InstalmentNumber,
            Currency = quoteRequest.RequestedAmount.CurrencyCode
        };
        try
        {
            var res = await _apiQuoteService.ApiPostQuote(quote);
            return Ok(res);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}