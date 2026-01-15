using MortgageComparer.Entities;
using MortgageComparer.Models;

namespace MortgageComparerAPI.Models;

public class PostOfferRequest
{
    public int QuoteId { get; set; }
    public ApiUserEntity User { get; set; }
}