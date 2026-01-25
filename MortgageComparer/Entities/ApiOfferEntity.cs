using MortgageComparer.Models;
using MortgageComparer.StatesMachine;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities;

public class ApiOfferEntity : BasicOfferEntity
{
    public virtual ApiUserEntity User { get; set; }
    public double Percentage { get; set; }
    public int MonthlyInstallementAmount { get; set; }
    public string MonthlyInstallementCurrency { get; set; } = "PLN";
    
    public int RequestedAmount { get; set; }
    public string RequestedCurrency { get; set; }
    public int RequestedPeriodInMonths { get; set; }
    public string DocumentKey { get; set; }
    public byte[]? Contract { get; set; }
    public byte[]? SignedContract { get; set; }
}