using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MortgageComparer.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Entities;

namespace MortgageComparer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("me")]
        public async Task<ActionResult<UserEntity>> GetMyProfileAsync()
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
            return Ok(new 
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                HasBirthDate = user.DateOfBirth != null,
            });
        }

        [HttpPost("BirthDate")]
        public async Task<ActionResult> CheckForTheBirthDateAsync([FromBody] UserBirthDateDto user)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            UserEntity foundUser = await _context.Users.FindAsync(userId);
            if (foundUser == null)
            {
                return NotFound("Nie znaleziono.");
            }
            // PostgreSQL przyjmuje tylko UTC time
            foundUser.DateOfBirth = user.BirthDate.ToUniversalTime();
            await _context.SaveChangesAsync();
            return Ok("Zaktualizowano datę");
        }
    }

    public class UserBirthDateDto
    {
        public DateTime BirthDate { get; set; }
    }
}