using MortgageComparer.Models;

namespace MortgageComparerAPI.Models;

public class QuoteResponse
{
    public int QuoteId { get; set; }
    public Money TotalAmountToPay { get; set; }
}