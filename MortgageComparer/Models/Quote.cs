namespace MortgageComparer.Models;

public class Quote
{
    public int Id { get; set; }
    public int RequestedAmount { get; set; }
    public int AmountToPay { get; set; }
    public int Installments { get; set; }
    public string Currency { get; set; } = "PLN";
    public DateTime CreatedAt { get; set; }
}