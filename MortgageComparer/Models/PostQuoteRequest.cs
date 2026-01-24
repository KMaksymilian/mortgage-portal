using MortgageComparer.Data;

namespace MortgageComparer.Models;

public class PostQuoteRequest{
    public MoneyModel RequestedAmount { get; set; }
    public int InstalmentNumber { get; set; }
    
}

