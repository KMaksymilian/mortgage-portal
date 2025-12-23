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

    public DateTime? DateOfBirth { get; set; }

    public JobDetailsModel? JobDetails { get; set; }

    [ForeignKey(nameof(JobDetails.JobTypeId))]
    public virtual JobTypeEntity? JobType { get; set; }
    
    public PersonalDocumentModel? PersonalDocument { get; set; }

    [ForeignKey(nameof(PersonalDocument.TypeId))]
    public PersonalDocumentTypeEntitycs? PersonalDocumentType { get; set; }

}