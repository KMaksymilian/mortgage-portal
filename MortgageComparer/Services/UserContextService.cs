using System.Net;
using System.Security.Claims;
using MortgageComparer.Services.Interfaces;

namespace MortgageComparer.Services;

public class UserContextService : IUserContextService
{
    IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return null;
        }
        return userId;
    }
}