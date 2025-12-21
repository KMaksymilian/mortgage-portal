using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities {

    [Table("PersonalDocumentType")]
    public class PersonalDocumentTypeEntitycs {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
