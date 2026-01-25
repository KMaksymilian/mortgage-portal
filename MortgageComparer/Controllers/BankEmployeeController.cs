using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.StateMachine;
using MortgageComparer.StatesMachine;

namespace MortgageComparer.Controllers {

    

    [Authorize(Roles = "BankEmployee")]
    [ApiController]
    [Route("api/bank/offers")]
    public class BankEmployeeController : ControllerBase {
        private readonly IOfferService _offerService;

        public BankEmployeeController(IOfferService offerService) {
            _offerService = offerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOffersAsync() => 
            Ok(await _offerService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfferAsync(int id) =>   
            await _offerService.GetByIdAsync(id) is { } offer ? Ok(offer) : NotFound();

        [HttpPost("{id}/execute")]
        public async Task<IActionResult> ExecuteActionAsync(int id, [FromBody] ActionRequest request) {
            try {
                var action = OfferActionFactory.Create(request);

                return await _offerService.ExecuteActionAsync(id, action)
                    ? Ok(new { Message = $"Action {request.Action} executed successfully." })
                    : NotFound($"Offer with ID {id} not found.");
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception) {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
