using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MortgageComparer.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Entities;
using MortgageComparer.Services.Interfaces;

namespace MortgageComparer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfileAsync()
        {

            UserProfileDto user;
            try
            {
                user = await _userService.GetProfileAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("BirthDate")]
        public async Task<ActionResult> CheckForTheBirthDateAsync([FromBody] UserBirthDateDto user)
        {
            try
            {
                await _userService.UpdateBirthdayAsync(user);
                return Ok("Zaktualizowano datę");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddUserAsync([FromBody] UserEntity? user)
        {
            if (user == null)
            {
                return BadRequest("Brak danych użytkownika");
            }

            UserEntity newUser;
            try
            {
                newUser = await _userService.AddUserAsync(user);
                return Ok(newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class UserBirthDateDto
    {
        public DateTime BirthDate { get; set; }
    }

    public class UserProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool HasBirthDate { get; set; } 
    }
}