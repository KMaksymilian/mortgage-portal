using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities {
    [Table("JobTypes")]
    public class JobTypeEntity {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
