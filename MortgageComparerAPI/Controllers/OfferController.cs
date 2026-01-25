using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Services.Interfaces;
using MortgageComparerAPI.Models;
using MortgageComparerAPI.Services;

namespace MortgageComparerAPI.Controllers;

[ApiController]
[Route("api/[controller]/x")]
[Authorize]
public class OfferController : ControllerBase
{
    private readonly IApiOfferService  _offerService;

    public OfferController(IApiOfferService offerService)
    {
        _offerService = offerService;
    }
    [HttpPost("quote/{quoteId}")]
    public async Task<IActionResult> PostOfferAsync([FromBody] PostOfferRequest request, int quoteId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        try
        {
            var res = await _offerService.PostOfferAsync(request, userId, quoteId);
            return Ok(res);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("{offerId}")]
    public async Task<IActionResult> GetOfferAsync(int offerId)
    {
        try
        {
            var res = await _offerService.GetOfferByIdAsync(offerId);
            return Ok(res);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{offerId}/document/{key}")]
    public async Task<IActionResult> GenerateContractAsync(int offerId, string key)
    {
        try
        {
            var res = await _offerService.GetContractAsync(offerId, key);
            return File(res.FileContents, res.ContentType, res.FileName);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{offerId}/document/{key}/upload")]
    public async Task<IActionResult> UploadContractAsync([FromForm] IFormFile file, int offerId, string key)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest();
        }
        try
        {
            await _offerService.PostContractAsync(file, offerId, key);
            return Ok();
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}