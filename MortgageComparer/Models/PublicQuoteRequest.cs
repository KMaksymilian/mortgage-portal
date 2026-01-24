namespace MortgageComparer.Models;

public class PublicQuoteRequest
{
    public decimal Amount { get; set; }
    public int Months { get; set; }
    public decimal OwnContribution { get; set; }
}