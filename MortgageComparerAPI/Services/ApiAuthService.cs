using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparerAPI.Models;

namespace MortgageComparerAPI.Services;

public class ApiAuthService : IApiAuthService
{
    private readonly IConfiguration _configuration;
    private AppDbContext _dbContext;

    public ApiAuthService(IConfiguration configuration,  AppDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }
    public async Task<ApiLoginResponse> Authenticate(ApiLoginRequest request)
    {
        if (request.ClientSecret != "12345")
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }
        if (!request.Email.Contains("@"))
        {
            throw new UnauthorizedAccessException("Wrong email");
        }
        var user = _dbContext.OurApiUsers.FirstOrDefault(u => u.Email == request.Email);
        if (user == null)
        {
            user = new ApiUserEntity()
            {
                Email = request.Email,
            };
            await _dbContext.OurApiUsers.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }
        var stringKey = _configuration["Jwt:SecretKey"];
        var key = Encoding.UTF8.GetBytes(stringKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email,  user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtString = tokenHandler.WriteToken(token);
        return new ApiLoginResponse()
        {
            AccessToken = jwtString,
            ExpiresIn = tokenDescriptor.Expires.Value.ToString("o")
        };
    }
}