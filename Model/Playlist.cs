using System.ComponentModel.DataAnnotations;

namespace purpuraMain.Model;

public class Playlist
{
  public Guid Id { get; set; } = Guid.NewGuid();

  [MinLength(2)]
  public required string Name { get; set; }
  public string Description { get; set; } = "";

  public required string UserId { get; set; }
  public required User User { get; set; }

  public string ImageUrl { get; set; } = "https://res.cloudinary.com/dw43hgf5p/image/upload/v1734735036/pskgqakw7ojmfkn7x076.jpg";

  public required bool IsPublic { get; set; }
  public required bool Editable { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime LastUpdated { get; set; }
  public ICollection<Song> Songs { get; set; } =[];
  public ICollection<Library> Libraries { get; set; } =[];

}