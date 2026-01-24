using MortgageComparer.Data;

namespace MortgageComparer.Models;

public class PostQuoteRequest{
    public MoneyDto RequestedAmount { get; set; }
    public int InstalmentNumber { get; set; }

    public PostQuoteRequest(MoneyDto requestedAmount, int instalmentNumber)
    {
        this.RequestedAmount = requestedAmount;
        this.InstalmentNumber = instalmentNumber;
    }
}

