using Microsoft.AspNetCore.Mvc;

namespace MortgageComparer.Controllers.Interfaces {
    public interface ICreateController {
        Task<ActionResult> AddAsync([FromBody] object? dto);
    }
}
