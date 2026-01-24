using MortgageComparer.DataTransferObjects;
using MortgageComparer.Models;

public class OfferDto {
    // Powiązanie z zapytaniem
    public required QuoteDto QuoteDto { get; set; }

    // Dane użytkownika potrzebne do wniosku
    public PersonalDataModel? PersonalDataModel { get; set; }
    public PersonalDocumentModel? PersonalDocumentModel { get; set; }
    public JobDetailsModel? JobDetails { get; set; }

    // Dane wyjściowe 
    public int OfferId { get; set; }
    public string? BankName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public double Percentage { get; internal set; }
    public DateTime? DocumentLinkValidDate { get; internal set; }
    public string? DocumentLink { get; internal set; }
    public DateTime? UpdatedAt { get; internal set; }
    public string? StatusDescription { get; internal set; }
    public string? ApprovedBy { get; internal set; }
}