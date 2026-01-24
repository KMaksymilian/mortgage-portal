using MortgageComparer.Models;
using MortgageComparer.StatesMachine;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities {
    public class BasicOfferEntity : BasicEntity{
        public int QuoteId { get; set; }
        [ForeignKey("QuoteId")]
        public virtual QuoteEntity Quote { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserEntity User { get; set; }

        public OfferStatus Status { get; set; } = OfferStatus.Created;
        public string? StatusDescription { get; set; }

        public string? DocumentLink { get; set; }
        public DateTime? ContractLinkValidDate { get; set; }
        public string? SingedBy { get; set; }
        public byte[]? ContractData { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
