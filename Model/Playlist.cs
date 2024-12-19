using System.ComponentModel.DataAnnotations;

namespace purpuraMain.Model;

public class Playlist
{
  public required string Id { get; set; }

  [MinLength(2)]
  public required string Name { get; set; }
  public string? Description { get; set; }

  public string? UserId { get; set; }
  public User? User { get; set; }

  public required bool IsPublic { get; set; }
  public DateTime CreatedAt { get; set; }
  public ICollection<Song>? Songs { get; set; } 
  public ICollection<Library>? Libraries { get; set; }

}