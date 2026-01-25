using Microsoft.EntityFrameworkCore;
using MortgageComparer.Data;
using MortgageComparer.DataTransferObjects;
using MortgageComparer.Entities;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.StateMachine;
using MortgageComparer.StatesMachine;
using System;

namespace MortgageComparer.Services {
    public class BankEmployeeOfferService : IOfferService {

        private readonly AppDbContext _context;

        public BankEmployeeOfferService(AppDbContext context) {
            _context = context;
        }

        public async Task<bool> ExecuteActionAsync(int offerId, IOfferAction action) {
            if (await GetEntityByIdAsync(offerId) is not { } offerEntity) {
                return false;
            }

            var stateMachine = new OfferStateMachine(offerEntity);
            action.Execute(stateMachine);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<OfferDto>> GetAllAsync() =>  
            await _context.Offers.Select(o => new OfferDto(o)).ToListAsync();

        public async Task<OfferDto?> GetByIdAsync(int id) =>
            (await GetEntityByIdAsync(id)) is { } e ? new OfferDto(e) : null;

        Task<IEnumerable<OfferDto>> IOfferService.CreateAsync(OfferDto offerDto) {
            throw new NotImplementedException();
        }

        private async Task<OfferEntity?> GetEntityByIdAsync(int id) =>  
            await _context.Offers.FirstOrDefaultAsync(o => o.Id == id);


    }
}
