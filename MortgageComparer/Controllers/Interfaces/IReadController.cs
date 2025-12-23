using Microsoft.AspNetCore.Mvc;

namespace MortgageComparer.Controllers.Interfaces {
    public interface IReadController<TListDto, TDetailDto, TKey> {
        Task<ActionResult<IEnumerable<TListDto>>> GetAll();
        Task<ActionResult<TDetailDto>> GetById([FromRoute] TKey id);

    }

}
