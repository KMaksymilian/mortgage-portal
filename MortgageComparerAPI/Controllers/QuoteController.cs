using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MortgageComparerAPI.Models;
using MortgageComparerAPI.Services;

namespace MortgageComparerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuoteController : ControllerBase
{
    private readonly IApiQuoteService _apiQuoteService;

    public QuoteController(IApiQuoteService apiQuoteService)
    {
        _apiQuoteService = apiQuoteService;
    }

    [HttpPost("quote")]
    public async Task<IActionResult> QuotePostAsync([FromBody] QuoteRequest quoteRequest)
    {
        try
        {
            var res = await _apiQuoteService.ApiPostQuote(quoteRequest);
            return Ok(res);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}