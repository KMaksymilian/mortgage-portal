using System.Text.Json.Serialization;

namespace MortgageComparer.Models;

public class PostOfferResponse
{
    public int QuoteId { get; set; }
    public string BankName { get; set; }
    public int InternalId { get; set; }
    [JsonPropertyName("offerId")]
    public int OfferId { get; set; }
    [JsonPropertyName("instalmentAmount")]
    public MoneyDto InstalementAmount { get; set; }
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }
    public string DocumentLink { get; set; }
    public string? DocumentLinkValidDate { get; set; }
    public double Percentage { get; set; }
}