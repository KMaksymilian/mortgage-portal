using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace MortgageComparer.Models {
    
    [Owned]
    public class JobDetailsModel {
        [JsonPropertyName("typeId")]
        public int JobTypeId { get; set; }
        [JsonPropertyName("jobStartDate")]
        public DateTime StartDate { get; set; }
        [JsonPropertyName("jobEndDate")]
        public DateTime EndDate { get; set; }
        public required MoneyDto Income { get; set; }
    }
}
