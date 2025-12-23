using Microsoft.EntityFrameworkCore;
using MortgageComparer.Models;
using MortgageComparer.StatesMachine;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities;

[Table("Offers")]
public class OfferEntity : BasicEntity {

    // --- Foreign Keys ---
    public int ApplicantId { get; set; }

    [ForeignKey(nameof(ApplicantId))]
    public virtual UserEntity? Applicant { get; set; }


    public int QuoteId { get; set; }

    [ForeignKey(nameof(QuoteId))]
    public virtual QuoteEntity? Quote { get; set; }

    [MaxLength(50)]
    public string? ExternalBankOfferId { get; set; }


    // --- State ---
    public OfferStatus Status { get; set; } = OfferStatus.Created;

    [MaxLength(500)]
    public string? StatusDescription { get; set; }


    // --- OFFER DETAILS ---
    public int? BankPercentage { get; set; }
    public required MoneyModel RequestedMoney { get; set; }
    public MoneyModel? MonthlyInstallment { get; set; }

    // --- Documents ---

    [MaxLength(500)]
    public string? DocumentLink { get; set; }
    public DateTime? ContractLinkValidDate { get; set; }
    public string? SingedBy { get; set; }

    // --- Timestamps ---
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdateDate { get; set; }
}