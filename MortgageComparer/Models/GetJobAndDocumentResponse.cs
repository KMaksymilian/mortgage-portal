using MortgageComparer.DataTransferObjects;
using MortgageComparer.Services;

namespace MortgageComparer.Models;

public class GetJobAndDocumentResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public DocumentDto Job { get; set; }
    public DocumentDto Document { get; set; }
}
