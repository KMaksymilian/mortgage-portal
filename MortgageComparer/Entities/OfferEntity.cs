using Microsoft.EntityFrameworkCore;
using MortgageComparer.Models;
using MortgageComparer.StatesMachine;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities;

[Table("Offers")]
public class OfferEntity : BasicEntity {
    
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual UserEntity? User { get; set; }
    public int QuoteId { get; set; }
    

    [MaxLength(50)]
    public string? ExternalBankOfferId { get; set; }

    public OfferStatus Status { get; set; } = OfferStatus.Created;

    [MaxLength(500)]
    public string? StatusDescription { get; set; }
    
    public int? BankPercentage { get; set; }
    public required MoneyModel RequestedMoney { get; set; }
    public MoneyModel? MonthlyInstallment { get; set; }
    

    [MaxLength(500)]
    public string? DocumentLink { get; set; }
    public DateTime? ContractLinkValidDate { get; set; }
    public string? SingedBy { get; set; }
    
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdateDate { get; set; }
}