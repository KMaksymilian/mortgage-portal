using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MortgageComparer.Models;

namespace MortgageComparer.Entities;

public class ApiOfferEntity
{
    [Key]
    public int Id { get; set; }
    public int QuoteId { get; set; }
    [ForeignKey("QuoteId")]
    public virtual Quote Quote { get; set; }
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual ApiUserEntity User { get; set; }
    public double Percentage { get; set; }
    public int MonthlyInstallementAmount { get; set; }
    public string MonthlyInstallementCurrency { get; set; }
    public int RequestedAmount { get; set; }
    public string RequestedCurrency { get; set; }
    public int RequestedPeriodInMonths { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string DocumentKey { get; set; }
    public byte[]? Contract { get; set; }
    public byte[]? SignedContract { get; set; }
}