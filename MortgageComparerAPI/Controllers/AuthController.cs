using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MortgageComparerAPI.Models;
using MortgageComparerAPI.Services;
using static System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler;

namespace MortgageComparerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IApiAuthService _apiAuthService;

    public AuthController(IApiAuthService apiAuthService)
    {
        _apiAuthService = apiAuthService;
    }
    [HttpPost]
    public IActionResult Auth([FromBody] ApiLoginRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.ClientSecret))
        {
            return BadRequest("Bad Request");
        }

        try
        {
            var res = _apiAuthService.Authenticate(request);
            return Ok(res);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex);
        }
    }
}