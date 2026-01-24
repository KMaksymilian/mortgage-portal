using MortgageComparer.Services;

namespace MortgageComparer.Models;

public class AnonymousQuoteResponse
{
    public int QuoteId { get; set; } // To ID, które frontend zapisze do "Dokończ wniosek"
    public List<OfferSummaryDto> Offers { get; set; } // Lista do wyświetlenia
}