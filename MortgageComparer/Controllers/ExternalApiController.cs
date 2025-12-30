using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace MortgageComparer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExternalApiController : ControllerBase
{
    private AppDbContext _context;
    private IConfiguration _configuration;
    private int QuoteId { get; set; }

    public ExternalApiController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;

        _configuration = configuration;
    }

    [Authorize]
    [HttpPost("Quote")]
    public async Task<IActionResult> GetExternalApiOfferAsync([FromBody] CalculatorRequestModel offer)
    {
        if (offer == null || offer.RequestedAmount == null)
        {
            return BadRequest("Invalid data");
        }

        int? userId = GetUserRegisteredStatus();
        if (userId == null)
        {
            return Unauthorized("Użytkownik nie jest zalogowany.");
        }
        
        using var client = new HttpClient();
        var tokenDto = await GetTokenResponseAsync();
        

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto);

        var apiResponse = await client.PostAsJsonAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Quote", offer);

        var result = await apiResponse.Content.ReadAsStringAsync();
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<ExternalRequestResponse>(result, jsonSettings);
        OfferEntity offerEntity = new OfferEntity
        {
            UserId = (int)userId,
            QuoteId = response.QuoteId,
            RequestedMoney = offer.RequestedAmount
        };
        _context.Offers.AddAsync(offerEntity);
        await _context.SaveChangesAsync();
        return Content(result, "application/json");
    }

    [Authorize]
    [HttpGet("DocumentAndJobTypes")]
    public async Task<IActionResult> GetGovernmentDocumentTypesAndJobTypeAsync()
    {
        int? userId = GetUserRegisteredStatus();
        if (userId == null)
        {
            return Unauthorized("Użytkownik nie jest zalogowany.");
        }

        var user = await _context.Users
            .Include(u => u.JobType)
            .Include(u => u.PersonalDocument) 
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound();
        }

        bool dataChanged = false;
        if (user.JobType == null)
        {
            JobTypeEntity userJob = await GetJobTypesAsync();
            var isInDataBase = await _context.JobTypes.FindAsync(userJob.JobTypeId);
            user.JobType = isInDataBase ?? userJob;
            dataChanged = true;
        }

        if (user.PersonalDocument == null)
        {
            PersonalDocumentTypeEntity userDocument = await GetDocumentsAsync();
            var isInDataBase = await _context.DocumentTypes.FindAsync(userDocument.PersonalDocumentId);
            user.PersonalDocument = isInDataBase ?? userDocument;
            dataChanged = true;
        }

        if (dataChanged)
        {
            await _context.SaveChangesAsync();
        }
        var response = new 
        {
            firstName = user.FirstName,
            lastName = user.LastName,
            email = user.Email,
            birthDate = user.DateOfBirth,
            job = new {
                name = user.JobType?.Name,
                description = user.JobType?.Description
            },
            document = new {
                name = user.PersonalDocument?.Name,
                description = user.PersonalDocument?.Description
            }
        };

        return Ok(response);
    }

    public async Task<PersonalDocumentTypeEntity> GetDocumentsAsync()
    {
        var tokenDto =  await GetTokenResponseAsync();
        using var client = new HttpClient();
        var documentApiResponse = 
            await client.GetAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Dictionary/governmentDocumentTypes");

        var documentJsonResponse = await documentApiResponse.Content.ReadAsStringAsync();
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var documentResult = JsonSerializer.Deserialize<List<PersonalDocumentTypeEntity>>(documentJsonResponse, jsonSettings);
        int count = documentResult.Count;
        int randomNumber = new Random().Next(0, count);
        PersonalDocumentTypeEntity userJob = new PersonalDocumentTypeEntity()
        {
            PersonalDocumentId = documentResult[randomNumber].PersonalDocumentId,
            Name = documentResult[randomNumber].Name,
            Description = documentResult[randomNumber].Description
        };
        return userJob;
    }

    public async Task<JobTypeEntity> GetJobTypesAsync()
    {
        var tokenDto =  await GetTokenResponseAsync();
        using var client = new HttpClient();
        var jobApiResponse = 
            await client.GetAsync("https://mini.loanbank.api.snet.com.pl/api/v1/Dictionary/jobTypes");

        var jobJsonResponse = await jobApiResponse.Content.ReadAsStringAsync();
        var jsonSettings = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var jobResult = JsonSerializer.Deserialize<List<JobTypeEntity>>(jobJsonResponse, jsonSettings);
        int count = jobResult.Count;
        int randomNumber = new Random().Next(0, count);
        JobTypeEntity userJob = new JobTypeEntity
        {
            JobTypeId = jobResult[randomNumber].JobTypeId,
            Name = jobResult[randomNumber].Name,
            Description = jobResult[randomNumber].Description
        };
        return userJob;
    }

    public async Task<string?> GetTokenResponseAsync()
    {

        var tokenUrl = "https://indentitymanager.snet.com.pl/connect/token";
        var clientId = _configuration["ExternalApi:Login"];
        var clientSecret = _configuration["ExternalApi:Secret"];

        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "scope", "MiNI.LoanBank.API" }
        };
        var content = new FormUrlEncodedContent(requestData);
        using var client = new HttpClient();
        var response =  await client.PostAsync(tokenUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        return token?.AccessToken;
    }

    public int? GetUserRegisteredStatus()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return null;
        }
        return userId;
    }
}