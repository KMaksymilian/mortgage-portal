namespace MortgageComparer.Models;

public class FinalizeQuoteRequest
{
    public int QuoteId { get; set; }
    
    public decimal Earnings { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime JobStartDate { get; set; }
    public DateTime? JobEndDate { get; set; }
}