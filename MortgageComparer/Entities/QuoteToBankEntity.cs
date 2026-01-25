using MortgageComparer.Migrations;

namespace MortgageComparer.Entities {
    public class QuoteToBankEntity : BasicEntity {
        public int QuoteId { get; set; }
        public required string BankCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
