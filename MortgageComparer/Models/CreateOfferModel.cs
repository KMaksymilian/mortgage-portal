using MortgageComparer.Entities;
using System.ComponentModel.DataAnnotations;

namespace MortgageComparer.Models {
    public class CreateOfferModel {
        [Required]
        public int QuoteId { get; set; } 

        public required UserEntity PersonalData { get; set; }

        public required PersonalDocumentModel PersonalDocument { get; set; }

        public required JobDetailsModel JobDetails { get; set; }
    }
}
