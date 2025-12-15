using System.ComponentModel.DataAnnotations;
using MortgageComparer.Services;

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
     public DateTime? DateOfBirth { get; set; }
     //public List<IOffer> Offers { get; set; } = new  List<IOffer>();
     public User() 
     {
         FirstName = string.Empty;
         LastName = string.Empty;
         Email = string.Empty;
         DateOfBirth = null;
     }
 
     public User(string firstName, string lastName, string email)
     {
         this.FirstName = firstName;
         this.LastName = lastName;
         this.Email = email;
         this.DateOfBirth = null;
     }

     public User(int id, string firstName, string lastName, string email)
     {
         this.Id = id;
         this.FirstName = firstName;
         this.LastName = lastName;
         this.Email = email;
         this.DateOfBirth = null;
     }
}