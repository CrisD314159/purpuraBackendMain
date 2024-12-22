namespace purpuraMain.Model;

public class Library
{
  public required string Id { get; set; }
  public  string? UserId { get; set; }
  public User? User { get; set; }

  public ICollection<Playlist> Playlists { get; set; } = [];
  public ICollection<Song> Songs { get; set; } = [];
  public ICollection<Album> Albums { get; set; } = [];
}