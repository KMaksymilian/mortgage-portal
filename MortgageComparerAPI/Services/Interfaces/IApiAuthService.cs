using MortgageComparerAPI.Models;

namespace MortgageComparerAPI.Services;

public interface IApiAuthService
{
    public Task<ApiLoginResponse> Authenticate(ApiLoginRequest request);
}