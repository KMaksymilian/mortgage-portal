using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MortgageComparer.Models;
using MortgageComparer.Data;

namespace MortgageComparer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }
    [HttpPost]
    public async Task<ActionResult> AddUserAsync([FromBody] User? user)
    {
        if (user == null)
        {
            return BadRequest("Brak danych użytkownika");
        }
        bool exists = _context.Users.Any((u) => u.Email == user.Email);
        if (exists)
        {
            return Conflict("Użytkownik o tych samych danych istnieje");
        }
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return Ok(user);
    }
}