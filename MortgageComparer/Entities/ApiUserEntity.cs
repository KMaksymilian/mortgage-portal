using MortgageComparer.Models;

namespace MortgageComparer.Entities;

public class ApiUserEntity
{
    public PersonalDataModel PersonalData { get; set; }
    public PersonalDocumentModel GovernmentDocument { get; set; }
    public JobDetailsModel JobDetails { get; set; }
}