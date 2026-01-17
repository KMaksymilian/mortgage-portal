namespace MortgageComparer.Models;

public class PostQuoteResponse
{
    public string BankName { get; set; }
    public int QuoteId { get; set; }
    public MoneyDto InstalmentAmount { get; set; }
    public DateTime CreatedDate { get; set; }
}