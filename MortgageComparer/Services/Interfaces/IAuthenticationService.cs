using MortgageComparer.Models;

namespace MortgageComparer.Services.Interfaces;

public interface IAuthenticationService
{
    public Task<LoginResponseDto> GetGoogleTokenAsync(GoogleLoginRequestModel request);
    public Task<LoginResponseDto> GetGoogleAdminTokenAsync(GoogleLoginRequestModel request);
}