using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities {
    public class PersonalDocumentTypeEntity {
        [Key]
        public int PersonalDocumentId { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
    }
}
