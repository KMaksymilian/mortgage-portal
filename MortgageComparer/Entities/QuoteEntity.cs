using Microsoft.EntityFrameworkCore;
using MortgageComparer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities {

    [Table("Quotes")]
    public class QuoteEntity : BasicEntity {
        public int QuoteId { get; set; }
        public string BankName { get; set; }
        public int? ExternalQuoteId { get; set; }
        public MoneyDto InstalmentAmount { get; set; }
        public decimal RequestedAmount { get; set; }
        public int Months { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    }
}
