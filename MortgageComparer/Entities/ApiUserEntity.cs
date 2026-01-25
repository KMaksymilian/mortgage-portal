using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MortgageComparer.Models;
using MortgageComparerAPI.Models;

namespace MortgageComparer.Entities;

public class ApiUserEntity
{
    [Key]
    public int Id { get; set; }
    public string Email { get; set; }
    public string? FirstName { get; set; } = null!;
    public string? LastName { get; set; } = null!;
    public string? BirthDate { get; set; }
    public int? DocTypeId { get; set; }
    
    [MaxLength(50)]
    public string? Number { get; set; }
    [JsonPropertyName("typeId")]
    public int? JobTypeId { get; set; }
    [JsonPropertyName("jobStartDate")]
    public DateTime? StartDate { get; set; }
    [JsonPropertyName("jobEndDate")]
    public DateTime? EndDate { get; set; }
    public MoneyDto? Income { get; set; }

    public void UpdateUser(PostOfferRequest request)
    {
        this.FirstName = request.PersonalData.FirstName;
        this.LastName = request.PersonalData.LastName;
        this.BirthDate = request.PersonalData.BirthDate;
        this.DocTypeId = request.GovernmentDocument.TypeId;
        this.Number = request.GovernmentDocument.Number;
        this.JobTypeId = request.JobDetails.JobTypeId;
        this.StartDate = request.JobDetails.StartDate;
        this.EndDate = request.JobDetails.EndDate;
        this.Income = request.JobDetails.Income;
    }
}