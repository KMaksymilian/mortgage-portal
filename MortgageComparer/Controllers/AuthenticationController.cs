using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MortgageComparer.Data;
using MortgageComparer.Models;

namespace MortgageComparer.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthenticationController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("google-login")]
    public async Task<ActionResult> GetGoogleTokenAsync([FromBody] GoogleLoginRequest request)
    {
        GoogleJsonWebSignature.Payload validPayload;
        try
        {
            validPayload = await GoogleJsonWebSignature.ValidateAsync(request.Token);
        }
        catch (InvalidJwtException)
        {
            return BadRequest("Invalid Google Token");
        }
        if (validPayload == null)
        {
            return BadRequest("Invalid Google Token");
        }
        User? user = await _context.Users.FirstOrDefaultAsync((u) => u.Email == validPayload.Email);
        if (user == null)
        {
            user = new  User
            {
                FirstName = validPayload.GivenName, LastName = validPayload.FamilyName,
                Email = validPayload.Email, GoogleUserId = validPayload.Subject
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        var jwtToken = GenerateJwtToken(user);
        return Ok(new
        {
            Token = jwtToken,
            email = validPayload.Email,
        });
    }
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            }),
            Expires = DateTime.UtcNow.AddHours(1), 
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}