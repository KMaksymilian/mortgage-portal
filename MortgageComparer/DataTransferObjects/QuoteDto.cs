using MortgageComparer.Models;

namespace MortgageComparer.DataTransferObjects {
    public class QuoteDto {
        public string BankName { get; set; } = string.Empty;
        public int Id { get; set; }
        public MoneyModel RequestedAmount { get; set; } = new MoneyModel();
        public MoneyModel InstallmentAmount { get; set; } = new MoneyModel();
        public int InstalmentNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
