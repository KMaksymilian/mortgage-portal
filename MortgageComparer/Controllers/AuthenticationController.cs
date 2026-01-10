using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services.Interfaces;

namespace MortgageComparer.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IExternalApiService _externalApiService;

    public AuthenticationController(AppDbContext context, IConfiguration configuration,  IExternalApiService externalApiService)
    {
        _context = context;
        _configuration = configuration;
        _externalApiService = externalApiService;
    }
    
    [HttpPost("google-login")]
    public async Task<ActionResult> GetGoogleTokenAsync([FromBody] GoogleLoginRequestModel request)
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
        UserEntity? user = await _context.Users.FirstOrDefaultAsync((u) => u.Email == validPayload.Email);
        if (user == null)
        {
            user = new  UserEntity
            {
                FirstName = validPayload.GivenName, LastName = validPayload.FamilyName,
                Email = validPayload.Email,
            };
            // zmockowane dane, to do: zmienić to
            if (user.JobType == null)
            {
                JobTypeEntity userJob = await _externalApiService.GetJobTypesAsync();
                var isInDataBase = await _context.JobTypes.FindAsync(userJob.JobTypeId);
                user.JobType = isInDataBase ?? userJob;
            }

            if (user.PersonalDocument == null)
            {
                PersonalDocumentTypeEntity userDocument = await _externalApiService.GetDocumentTypesAsync();
                var isInDataBase = await _context.DocumentTypes.FindAsync(userDocument.PersonalDocumentId);
                user.PersonalDocument = isInDataBase ?? userDocument;
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        var jwtToken = GenerateJwtToken(user);
        return Ok(new
        {
            Token = jwtToken,
            email = validPayload.Email,
            hasBirthDate = user.DateOfBirth != null,
        });
    }
    private string GenerateJwtToken(UserEntity user)
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