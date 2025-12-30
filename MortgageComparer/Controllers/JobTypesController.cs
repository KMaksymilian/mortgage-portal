using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Controllers.Interfaces;
using MortgageComparer.Services.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Data;
using MortgageComparer.Entities;
/*
using JobTypeEntity = MortgageComparer.Services.Interfaces.JobTypeEntity;
using OfferEntity = MortgageComparer.Services.Interfaces.OfferEntity;
using QuoteEntity = MortgageComparer.Services.Interfaces.QuoteEntity;
*/


namespace MortgageComparer.Controllers 
{
    [Route("api/[controller]")]
    public class DictionaryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IExternalApiService _externalApiService;
        private readonly IUserContextService _userContextService;
        public DictionaryController(AppDbContext context,  
            IExternalApiService externalApiService, IUserContextService userContextService) 
        {
            _context = context;
            _externalApiService = externalApiService;
            _userContextService = userContextService;
        }
        [Authorize]
        [HttpGet("DocumentAndJobTypes")]
        public async Task<IActionResult> GetGovernmentDocumentTypesAndJobTypeAsync()
        {
            int? userId = _userContextService.GetUserId();
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
                JobTypeEntity userJob = await _externalApiService.GetJobTypesAsync();
                var isInDataBase = await _context.JobTypes.FindAsync(userJob.JobTypeId);
                user.JobType = isInDataBase ?? userJob;
                dataChanged = true;
            }

            if (user.PersonalDocument == null)
            {
                PersonalDocumentTypeEntity userDocument = await _externalApiService.GetDocumentTypesAsync();
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
    }
}