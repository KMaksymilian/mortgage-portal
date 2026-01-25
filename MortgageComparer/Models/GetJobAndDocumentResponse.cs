using MortgageComparer.DataTransferObjects;
using MortgageComparer.Services;
using static MortgageComparer.Services.JobTypeService;

namespace MortgageComparer.Models;

public class GetJobAndDocumentResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public DocumentDto2 Job { get; set; }
    public DocumentDto2 Document { get; set; }
}
