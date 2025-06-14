namespace purpuraMain.Model;

public class Library
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required string UserId { get; set; }
  public required User User { get; set; }

  public ICollection<Playlist> Playlists { get; set; } = [];
  public ICollection<Song> Songs { get; set; } = [];
  public ICollection<Album> Albums { get; set; } = [];
}