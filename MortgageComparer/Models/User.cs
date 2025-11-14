using System.ComponentModel.DataAnnotations;
namespace MortgageComparer.Models;

public class User
{
    public int Id { get; set; }
    [Required]
     public string FirstName { get; set; }
     [Required]
     public string LastName { get; set; }
     [Required]
     public string Email { get; set; }
     public string GoogleUserId { get; set; }
     public User() 
     {
         FirstName = string.Empty;
         LastName = string.Empty;
         Email = string.Empty;
         GoogleUserId = string.Empty;
     }
 
     public User(string firstName, string lastName, string email, string googleUserId)
     {
         this.FirstName = firstName;
         this.LastName = lastName;
         this.Email = email;
         this.GoogleUserId = googleUserId;
     }
}