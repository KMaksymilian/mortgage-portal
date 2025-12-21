using System.ComponentModel.DataAnnotations;

namespace MortgageComparer.Models;

public class GoogleLoginRequestModel {
    [Required]
    public required string Token { get; set; }
    
}