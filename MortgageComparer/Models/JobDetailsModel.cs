using Microsoft.EntityFrameworkCore;

namespace MortgageComparer.Models {
    
    [Owned]
    public class JobDetailsModel {
        public int JobTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public required MoneyModel IncomeAmount { get; set; }
    }
}
