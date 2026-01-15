using MortgageComparer.Models;

namespace MortgageComparerAPI.Models;

public class PostOfferResponse
{
    public int OfferId { get; set; }
    public Money InstallmentAmount { get; set; }
    public DateTime CreateDate { get; set; }
}