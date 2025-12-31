using MortgageComparer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    // To do: Zaimplementować podawanie pensji oraz w czym zarabis przez użytkownika, na razie zmockowane dane

    public int Income { get; set; } = 10000;
    public string IncomeCurrencyCode { get; set; } = "PLN";

    public DateTime? DateOfBirth { get; set; }
    
    public int? JobTypeId { get; set; }
    [ForeignKey(nameof(JobTypeId))]
    public JobTypeEntity? JobType { get; set; }
    
    public int? DocumentId { get; set; }
    [ForeignKey(nameof(DocumentId))]
    public PersonalDocumentTypeEntity? PersonalDocument { get; set; }

}