using Microsoft.EntityFrameworkCore;
using MortgageComparer.Models;
using MortgageComparer.StatesMachine;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities;

[Table("Offers")]
public class OfferEntity : BasicOfferEntity {
    
    public string BankName { get; set; }

    [MaxLength(50)]
    public string? ExternalBankOfferId { get; set; }
    public byte[]? ContractData { get; set; }
    public string? FileName { get; set; }
    public string? FileContents { get; set; }
    public byte[]? SignedContractData { get; set; }
    public string? SignedFileName { get; set; }
    public string? SignedFileContents { get; set; }
    public double? BankPercentage { get; set; }
    public MoneyDto? RequestedMoney { get; set; }
    public MoneyDto? MonthlyInstallment { get; set; }
    

}