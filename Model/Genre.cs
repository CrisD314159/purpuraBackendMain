using Microsoft.EntityFrameworkCore;

namespace purpuraMain.Model;

[Index(nameof(Name), IsUnique = true)]
public class Genre
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public string Description { get; set; } = "";
  public string Color { get; set; } = "";
  public ICollection<Song> Songs { get; set; } = [];

}