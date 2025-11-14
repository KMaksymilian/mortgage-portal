using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MortgageComparer.Models;

namespace MortgageComparer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public ActionResult GetWelcomeMessage()
    {
        // Zwraca obiekt, który zostanie automatycznie zamieniony na JSON
        return Ok(new { message = "Witaj w moim API!" }); 
    }
}