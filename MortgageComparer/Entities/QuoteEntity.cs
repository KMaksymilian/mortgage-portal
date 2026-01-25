using Microsoft.EntityFrameworkCore;
using MortgageComparer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities {

    [Table("Quotes")]
    public class QuoteEntity : BasicEntity {
        public int QuoteId { get; set; }
        public MoneyModel InstalmentAmount { get; set; } = new MoneyModel();
        public MoneyModel RequestedAmount { get; set; } = new MoneyModel();
        public int Months { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    }
}
