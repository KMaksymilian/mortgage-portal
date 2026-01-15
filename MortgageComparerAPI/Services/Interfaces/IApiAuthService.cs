using MortgageComparerAPI.Models;

namespace MortgageComparerAPI.Services;

public interface IApiAuthService
{
    public ApiLoginResponse Authenticate(ApiLoginRequest request);
}