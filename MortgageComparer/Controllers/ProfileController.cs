using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MortgageComparer.Data;
using MortgageComparer.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MortgageComparer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("me")]
        public async Task<ActionResult<User>> GetMyProfileAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userIdString == null)
            {
                return Unauthorized();
            }
            
            var userId = int.Parse(userIdString);
            
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("Nie znaleziono użytkownika.");
            }
            
            return Ok(user);
        }
    }
}