using Microsoft.EntityFrameworkCore;
using MortgageComparer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities {

    [Table("Quotes")]
    public class QuoteEntity : BasicEntity {
        public int QuoteId { get; set; }
        public int RequestedMonths { get; set; }

        public required MoneyModel TotalAmountToPay { get; set; }

        [Range(1, 480)] 
        public int InstalmentNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int StatusId { get; set; }
        public string? StatusDescription { get; set; }
        
    }
}
