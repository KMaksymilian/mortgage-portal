using System.ComponentModel.DataAnnotations;

namespace MortgageComparer.Models;

public class GoogleLoginRequestModel
{
    [Required]
    public string Token { get; set; }
}