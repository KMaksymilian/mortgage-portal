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

namespace MortgageComparer.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    private readonly IExternalApiService _externalApiService;

    public AuthenticationService(IConfiguration configuration, AppDbContext context, IExternalApiService externalApiService)
    {
        _configuration = configuration;
        _context = context;
        _externalApiService = externalApiService;
    }
    public async Task<LoginResponseDto> GetGoogleTokenAsync(GoogleLoginRequestModel request)
    {
        GoogleJsonWebSignature.Payload validPayload;
        try
        {
            validPayload = await GoogleJsonWebSignature.ValidateAsync(request.Token);
        }
        catch (InvalidJwtException)
        {
            throw new Exception("Invalid Google Token");
        }
        if (validPayload == null)
        {
            throw new Exception("Invalid Google Token");
        }
        UserEntity? user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == validPayload.Email);
        
        if (user == null)
        {
            user = new UserEntity
            {
                FirstName = validPayload.GivenName,
                LastName = validPayload.FamilyName,
                Email = validPayload.Email,
                Income = null,
                DateOfBirth = null,
                DocumentId = 1,
                JobTypeId = 1,
                Role = "User"
            };
            if (user.JobType == null)
            {
                JobTypeEntity userJob = await _externalApiService.GetJobTypesAsync();
                var isInDataBase = await _context.JobTypes.FindAsync(userJob.Id);
                user.JobType = isInDataBase ?? userJob;
            }

            if (user.PersonalDocument == null)
            {
                PersonalDocumentTypeEntity userDocument = await _externalApiService.GetDocumentTypesAsync();
                var isInDataBase = await _context.DocumentTypes.FindAsync(userDocument.Id);
                user.PersonalDocument = isInDataBase ?? userDocument;
            }

            if (string.IsNullOrWhiteSpace(user.Role)) 
            {
                user.Role = "User";
                await _context.SaveChangesAsync();
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        
        var jwtToken = GenerateJwtToken(user);
        
        return new LoginResponseDto
        {
            Token = jwtToken,
            Email = user.Email,
            Earnings = user.Income,
            BirthDate = user.DateOfBirth,
            JobStartDate = user.JobStartDate,
            JobEndDate = user.JobEndDate,
            Role = user.Role
        };
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
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim("role", user.Role ?? "User")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<LoginResponseDto> GetGoogleAdminTokenAsync(GoogleLoginRequestModel request) {
        GoogleJsonWebSignature.Payload validPayload;
        try {
            validPayload = await GoogleJsonWebSignature.ValidateAsync(request.Token);
        }
        catch (InvalidJwtException) {
            throw new UnauthorizedAccessException("Invalid Google Token");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == validPayload.Email);

        // adminów NIE tworzymy automatycznie
        if (user == null) 
        {
            throw new UnauthorizedAccessException("Admin account does not exist");
        }

        if (!string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase)) 
        {
            throw new UnauthorizedAccessException("User is not an admin");
        }

        var jwtToken = GenerateJwtToken(user);

        return new LoginResponseDto {
            Token = jwtToken,
            Email = user.Email,
            Earnings = user.Income,
            BirthDate = user.DateOfBirth,
            JobStartDate = user.JobStartDate,
            JobEndDate = user.JobEndDate,
            Role = user.Role
        };
    }
}

public class LoginResponseDto
{
    public string Token { get; set; }
    public string Email { get; set; }
    public decimal? Earnings { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? JobStartDate { get; set; }
    public DateTime? JobEndDate { get; set; }
    public string Role { get; set; }
}