using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace purpuraMain.Model;

public class User : IdentityUser
{

  public DateTime CreatedAt { get; set; }

  public int CountryId { get; set; }

  public UserState State { get; set; }

  [Url]
  public required string ProfilePicture { get; set; }
  public required string VerificationCode { get; set; }
    public required UserRole Role { get; set; }

}