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

        var adminEmails = _configuration.GetSection("Admin:Emails").Get<string[]>() ?? Array.Empty<string>();
        var isAdmin = adminEmails.Any(e =>
            !string.IsNullOrWhiteSpace(e) &&
            string.Equals(e.Trim(), validPayload.Email, StringComparison.OrdinalIgnoreCase)
        );

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
                JobType = null, 
                PersonalDocument = null,
                Role = isAdmin ? "Admin" : "User"
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        if (isAdmin && !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase)) {
            user.Role = "Admin";
            await _context.SaveChangesAsync();
        }


        var jwtToken = GenerateJwtToken(user);
        if (user.JobType == null)
        {
            user.JobType = await _context.JobTypes.FirstOrDefaultAsync(j => j.Name == "Truck Driver");
        }

        if (user.DocumentId == null)
        {
            user.DocumentId  = 1;
        }
        
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

    public async Task<LoginResponseDto> GetGoogleAdminTokenAsync(GoogleLoginRequestModel request) 
    {
        var res = await GetGoogleTokenAsync(request);

        if (!string.Equals(res.Role, "Admin", StringComparison.OrdinalIgnoreCase)) 
        {
            throw new UnauthorizedAccessException("User is not an admin");
        }

        return res;
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
                new Claim(ClaimTypes.Role, user.Role)
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