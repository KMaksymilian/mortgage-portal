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


    public Task<List<OfferDto>> GetAllAsync() {
        throw new NotImplementedException();
    }

    public Task<OfferDto?> GetByIdAsync(int id) {
        throw new NotImplementedException();
    }

    public Task<bool> ExecuteActionAsync(int offerId, IOfferAction action) {
        throw new NotImplementedException();
    }

    Task<IEnumerable<OfferDto>> IOfferService.CreateAsync(OfferDto offerDto) {
        throw new NotImplementedException();
    }
}