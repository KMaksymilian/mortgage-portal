using MortgageComparer.Models;

namespace MortgageComparerAPI.Models;

public class ApiUserModel
{
    public PersonalDataModel PersonalData { get; set; }
    public PersonalDocumentModel GovernmentDocument { get; set; }
    public JobDetailsModel JobDetails { get; set; }
}