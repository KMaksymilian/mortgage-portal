using MortgageComparer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace MortgageComparer.Entities;

[Table("Users")]
public class UserEntity : BasicEntity {
    [Required]
    [MaxLength(100)]
    public required string FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public required string LastName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string Email { get; set; }

    public DateTime? DateOfBirth { get; set; }
    
    public int? JobTypeId { get; set; }
    [ForeignKey(nameof(JobTypeId))]
    public JobTypeEntity? JobType { get; set; }
    
    public int? DocumentId { get; set; }
    [ForeignKey(nameof(DocumentId))]
    public PersonalDocumentTypeEntity? PersonalDocument { get; set; }
    // To do: Zaimplementować to lepiej, na razie mockup
    public DateTime JobStartDate { get; set; } =  DateTime.UtcNow;
    public DateTime JobEndDate { get; set; } =  DateTime.UtcNow;
    public int Income { get; set; }= 10000;
    public string IncomeCurrCode { get; set; } = "PLN";


}