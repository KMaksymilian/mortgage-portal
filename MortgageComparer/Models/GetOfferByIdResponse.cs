namespace MortgageComparer.Models;

public class GetOfferByIdResponse
{
    public int Id { get; set; }
    public double Percentage { get; set; }
    public MoneyDto MonthlyInstallment { get; set; }
    public MoneyDto RequestedAmount { get; set; }
    public int RequestedPeriodInMonth { get; set; }
    public int? StatusId { get; set; }
    public string? StatusDescription { get; set; }
    public int? InquireId  { get; set; }
    public string CreateDate { get; set; }
    public string UpdateDate { get; set; }
    public string? ApprovedBy  { get; set; }
    public string? DocumentLink { get; set; }
    public string? DocumentLinkValidDate { get; set; }
}