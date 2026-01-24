using MortgageComparer.Models;

namespace MortgageComparer.Services.Interfaces;

public interface IAuthenticationService
{
    public Task<LoginResponseDto> GetGoogleTokenAsync(GoogleLoginRequestModel request);
}