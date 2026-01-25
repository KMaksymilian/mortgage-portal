using Azure.Core;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.BankLogic;
using MortgageComparer.Data;
using MortgageComparer.DataTransferObjects;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.StatesMachine;

namespace MortgageComparer.Services;

public class QuoteService : IQuoteService
{
    private readonly BankAggregator _banks;
    private AppDbContext _context;

    public QuoteService(BankAggregator banks,  AppDbContext context)
    {
        _banks = banks;
        _context = context;
    }

    public async Task<IEnumerable<QuoteDto>> PostQuoteAsync(QuoteDto quoteRequest) {
        var bankResponses = await _banks.PostQuotesFromAllBanksAsync(quoteRequest);
        if (bankResponses == null) {
            throw new Exception("No quotes found");
        }

        foreach (var response in bankResponses) {
            var quoteToBank = new QuoteToBankEntity {
                QuoteId = quoteRequest.Id, 
                BankCode = response.BankName, 
                Description = $"Oferta wygenerowana automatycznie: {DateTime.Now:yyyy-MM-dd}",
                CreatedAt = DateTime.UtcNow
                
            };

            _context.QuoteToBanks.Add(quoteToBank);
        }

        await _context.SaveChangesAsync();

        return bankResponses;
    }

    public async Task<QuoteDto?> GetQuoteById(int recordId) {
       
        var quoteEntity = await _context.QuoteToBanks
            .FirstOrDefaultAsync(q => q.Id == recordId);

        if (quoteEntity == null) {
            return null;
        }

        throw new NotImplementedException();
    }
}