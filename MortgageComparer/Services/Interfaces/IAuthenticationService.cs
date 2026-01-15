using MortgageComparer.Models;

namespace MortgageComparer.Services.Interfaces;

public interface IAuthenticationService
{
    public Task<GoogleLoginRequestModelDto> GetGoogleTokenAsync(GoogleLoginRequestModel request);
}