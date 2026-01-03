using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MortgageComparer.Models {

    [Owned]
    public class PersonalDocumentModel {
        public int TypeId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Number { get; set; }

        public string Description { get; set; }
    }
}
