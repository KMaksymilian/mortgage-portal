using System.Text.Json.Serialization;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using Newtonsoft.Json;

namespace MortgageComparerAPI.Models;

public class PostOfferRequest
{
    [JsonPropertyName("quoteId")]
    public int? QuoteId { get; set; }
    public PersonalDataModel PersonalData { get; set; }
    public PersonalDocumentModel GovernmentDocument { get; set; }
    public JobDetailsModel JobDetails { get; set; }
}