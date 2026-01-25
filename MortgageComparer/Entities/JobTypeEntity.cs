using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MortgageComparer.Entities {
    public class JobTypeEntity : BasicEntity {

        public required string Name { get; set; }
        public string Description { get; set; }
    }
}
