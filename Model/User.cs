using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace purpuraMain.Model;

public class User : IdentityUser
{

  [MinLength(8)]
  public required string Password { get; set; }

  [MinLength(2)]
  [MaxLength(30)]
  public required string Name { get; set; }


  public DateTime CreatedAt { get; set; }

  public int CountryId { get; set; }

  public UserState State { get; set; }

  [Url]
  public required string ProfilePicture { get; set; } 
  public required string VerificationCode { get; set; }

}