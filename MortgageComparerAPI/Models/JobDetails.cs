using MortgageComparer.Models;

namespace MortgageComparerAPI.Models;

public class JobDetails
{
    public int TypeId { get; set; }
    public DateTime JobStartDate { get; set; }
    public DateTime JobEndDate { get; set; }
    public Money Income { get; set; }
}