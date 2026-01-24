using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services.Interfaces;

namespace MortgageComparer.Controllers;

[Route("api/{controller}")]
public class QuoteController : ControllerBase
{
    private readonly IQuoteService _quoteService;
    private readonly IUserService _userService;

    public QuoteController(IQuoteService quoteService,  IUserService userService)
    {
        _quoteService = quoteService;
        _userService = userService;
    }
    [HttpPost("PublicQuote")]
    public async Task<IActionResult> PostQuoteAsync([FromBody] PublicQuoteRequest quoteRequest)
    {
        var res = await _quoteService.PostQuoteAsync(quoteRequest);
        return Ok(res);
    }
    [HttpPost("Finalize")]
    [Authorize]
    public async Task<IActionResult> FinalizeQuoteAsync([FromBody] FinalizeQuoteRequest request)
    {
        var userId = _userService.GetUserId();
        var offer = await _quoteService.FinalizeQuoteAsync(userId.Value, request);
        return Ok(offer);
    }
}