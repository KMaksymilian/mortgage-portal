using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MortgageComparerAPI.Models;

namespace MortgageComparerAPI.Controllers;

[ApiController]
[Route("api/[controller]/x")]
[Authorize]
public class ConfigController : ControllerBase
{
    [HttpGet("jobTypes")]
    public IActionResult GetJobTypes()
    {
        return Ok(_jobTypes.ToArray());
    }

    [HttpGet("docTypes")]
    public IActionResult GetDocumentTypes()
    {
        return Ok(_govermentDocumentTypes);
    }
    
    private readonly List<DocType> _jobTypes = new List<DocType>()
    {
        new DocType(1, "Accountant", "Accountant"),
        new DocType(2, "Actor", "Actor"),
        new DocType(3, "Architect", "Architect"),
        new DocType(4, "cosik ten", "cosik ten"),
    };
    
    private readonly List<DocType> _govermentDocumentTypes = new List<DocType>()
    {
        new DocType(1, "Id", "User id documet number"),
        new DocType(2, "Driver license", "User driver license number"),
        new DocType(3, "Passport", "User passport number"),
        new DocType(4, "Social number", "User social number"),
        new DocType(5, "mi bombini clatini", "bomba clat")
    };
}