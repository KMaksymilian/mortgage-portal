using MortgageComparer.DataTransferObjects;
using MortgageComparer.StateMachine;
using MortgageComparer.StatesMachine;

namespace MortgageComparer.Services.Interfaces {
    public interface IOfferService {
        Task<List<OfferDto>> GetAllAsync();
        Task<OfferDto?> GetByIdAsync(int id);
        Task<bool> ExecuteActionAsync(int offerId, IOfferAction action);
        Task<IEnumerable<OfferDto>> CreateAsync(OfferDto offerDto);
    }
}
