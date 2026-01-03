using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities {
    public class JobTypeEntity {
        [Key]
        public int JobTypeId { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
    }
}
