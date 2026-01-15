using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Services.Interfaces;
using MortgageComparerAPI.Models;
using MortgageComparerAPI.Services;

namespace MortgageComparerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OfferController : ControllerBase
{
    private readonly IApiOfferService  _offerService;

    public OfferController(IApiOfferService offerService)
    {
        _offerService = offerService;
    }
    [HttpPost]
    public async Task<IActionResult> PostOfferAsync([FromBody] PostOfferRequest request)
    {
        try
        {
            var res = await _offerService.PostOfferAsync(request);
            return Ok(res);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}