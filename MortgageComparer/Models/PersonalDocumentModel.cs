using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MortgageComparer.Models {

    [Owned]
    public class PersonalDocumentModel {
        public int TypeId { get; set; }
        public required string Number { get; set; }
        
    }
}
