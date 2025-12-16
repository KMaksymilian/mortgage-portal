namespace MortgageComparer.Services;

public interface IOffer
{
    public DateTime? StartDate { get; set; }
    public string BankName { get; set; }
    public double Percentage { get; set; }
    public decimal TotalLoanAmount { get; set; }
    public int MonthsToPay { get; set; }
}
