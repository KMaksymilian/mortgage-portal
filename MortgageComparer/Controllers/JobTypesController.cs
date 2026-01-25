using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Controllers.Interfaces;
using MortgageComparer.Services.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Data;
using MortgageComparer.Entities;


namespace MortgageComparer.Controllers 
{
    [Route("api/[controller]")]
    public class DictionaryController : ControllerBase
    {
        private readonly IJobTypeService _jobTypeService;
        public DictionaryController(IJobTypeService jobTypeService) 
        {
            _jobTypeService = jobTypeService;
        }
        [Authorize]
        [HttpGet("DocumentAndJobTypes")]
        public async Task<IActionResult> GetGovernmentDocumentTypesAndJobTypeAsync()
        {
            var response = await _jobTypeService.GetJobAndDocumentAsync();

            return Ok(response);
        }
    }
}