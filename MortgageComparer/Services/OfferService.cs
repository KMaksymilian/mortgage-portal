using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.BankLogic;
using MortgageComparer.Controllers;
using MortgageComparer.Data;
using MortgageComparer.DataTransferObjects;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.StateMachine;
using MortgageComparer.StatesMachine;
using MortgageComparerAPI.Models;

namespace MortgageComparer.Services;

public class OfferService : IOfferService
{
    private readonly BankAggregator _banks;
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IExternalApiService _externalApiService;
    private readonly IUserService _userService;
    public OfferService(AppDbContext context,  IHttpClientFactory httpClientFactory,
        IExternalApiService externalApiService,  IUserService userContextService,
        BankAggregator banks)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _externalApiService = externalApiService;
        _userService = userContextService;
        _banks = banks;
    }

   

    public async Task<List<OfferDto>> GetAllAsync() {
        int userId = _userService.GetUserId() ?? throw new Exception("User not found or not authenticated");
        var records = await _context.OfferToBanks.Where(ob => ob.UserId == userId).ToListAsync();
        List<OfferDto> offers = new List<OfferDto>();
        foreach (var record in records) {
            var offer = await _banks.GetOfferByIdFromSpecificBankAsync(record.BankCode, record.OfferId);
            if (offer != null) {
                offers.Add(offer);
            }
        }
        return offers;

    }

    public async Task<OfferDto?> GetByIdAsync(int id) {
        int userId = _userService.GetUserId() ?? throw new Exception("User not found");

        var record = await _context.OfferToBanks
            .FirstOrDefaultAsync(ob => ob.OfferId == id && ob.UserId == userId);

        if (record == null) { return null; } 

        return await _banks.GetOfferByIdFromSpecificBankAsync(record.BankCode, record.OfferId);
    }

    public Task<bool> ExecuteActionAsync(int offerId, IOfferAction action) {
        throw new NotImplementedException();
    }

    public async Task<OfferDto> CreateAsync(OfferDto offerDto) {

        int userId = _userService.GetUserId() ?? throw new Exception("User not found");

        var bankResponse = await _banks.PostOfferFromSpecificBankAsync(offerDto, offerDto.BankName);

        if (bankResponse == null) {
            throw new Exception("No bank responded");
        }

        var offerToBank = new OfferToBankEntity {
            UserId = userId,
            BankCode = bankResponse.BankName,
            OfferId = bankResponse.OfferId,
            StatusDescription = $"Oferta z dnia {DateTime.Now:g}",
            CreatedAt = DateTime.UtcNow
        };

        _context.OfferToBanks.Add(offerToBank);
        await _context.SaveChangesAsync();

        return bankResponse;
    }
}