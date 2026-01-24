using MortgageComparer.DataTransferObjects;
using MortgageComparer.StateMachine;
using MortgageComparer.StatesMachine;

namespace MortgageComparer.Services.Interfaces {
    public interface IOfferService2 {
        Task<List<OfferDto>> GetAllAsync();
        Task<OfferDto?> GetByIdAsync(int id);
        Task<bool> ExecuteActionAsync(int offerId, IOfferAction action);
    }
}
