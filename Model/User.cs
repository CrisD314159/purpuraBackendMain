using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace purpuraMain.Model;

[Index(nameof(Email), IsUnique = true)]
public class User
{
  public required string Id { get; set; }
  [EmailAddress]
  public required string Email { get; set; }

  [MinLength(8)]
  public required string Password { get; set; }

  [MinLength(2)]
  [MaxLength(30)]
  public string? Name { get; set; }
  public DateTime CreatedAt { get; set; }

  [MinLength(2)]
  [MaxLength(30)]
  public string? Phone { get; set; }

  public int CountryId { get; set; }
  public  Country? Country { get; set; }
  public UserState State { get; set; }
  public string? ProfilePicture { get; set; }
  public int VerifyCode { get; set; }

}