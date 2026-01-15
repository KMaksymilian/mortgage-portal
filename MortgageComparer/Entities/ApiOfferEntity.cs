using MortgageComparer.Models;

namespace MortgageComparer.Entities;

public class ApiOfferEntity
{
    public int Id { get; set; }
    public int QuoteId { get; set; }
    public double Percentage { get; set; }
    public int MonthlyInstallementAmount { get; set; }
    public string MonthlyInstallementCurrency { get; set; }
    public int RequestedAmount { get; set; }
    public string RequestedCurrency { get; set; }
    public int RequestedPeriodInMonths { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string DocumentKey { get; set; }
}