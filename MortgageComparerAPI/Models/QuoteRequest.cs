using System.ComponentModel.DataAnnotations;

namespace MortgageComparerAPI.Models;

public class QuoteRequest
{
    [Range(100, double.MaxValue, ErrorMessage = "Kwota musi być większa od 100 zł.")]
    public int Amount { get; set; }
    [Range(1, 120, ErrorMessage = "Liczba rat musi wynosić od 1 do 120.")]
    public int InstallmentsCount { get; set; }
    [AllowedValues("PLN", "USD", "GBP", "EUR", ErrorMessage = "Ziomek nie udzielamy kredytu w Twojej walucie")]
    public string Currency { get; set; } = "PLN";
}