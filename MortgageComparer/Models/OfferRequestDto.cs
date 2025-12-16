using MortgageComparer.Data;

namespace MortgageComparer.Models;

public class OfferRequestDto
{
    public Money RequestedAmount { get; set; }
    public int InstalmentNumber { get; set; }
}

public class Money
{
    public double Amount { get; set; }
    public string? CurrencyCode { get; set; }

    public Money(double amount, string? currencyCode)
    {
        this.Amount = amount;
        this.CurrencyCode = (currencyCode == null ? "PLN" : currencyCode);
    }
}