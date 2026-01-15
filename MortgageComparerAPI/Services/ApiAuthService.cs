using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MortgageComparerAPI.Models;

namespace MortgageComparerAPI.Services;

public class ApiAuthService : IApiAuthService
{
    private readonly IConfiguration _configuration;

    public ApiAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public ApiLoginResponse Authenticate(ApiLoginRequest request)
    {
        if (request.ClientSecret != "12345")
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }
        var settings = _configuration.GetSection("JwtToken");
        var stringKey = settings["SecretKey"];
        var key = Encoding.UTF8.GetBytes(stringKey);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "Druga grupa"),
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
            ExpiresIn = tokenDescriptor.Expires.Value.ToString("O")
        };
    }
}