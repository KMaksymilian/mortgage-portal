using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Services.Interfaces;

namespace MortgageComparer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private AppDbContext _context;
    private readonly IExternalApiService _externalApiService;

    public HomeController(AppDbContext context,  IExternalApiService externalApiService)
    {
        _context = context;
        _externalApiService = externalApiService;
    }
    [HttpPost]
    public async Task<ActionResult> AddUserAsync([FromBody] UserEntity? user)
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