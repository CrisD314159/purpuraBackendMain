namespace purpuraMain.Model;

public class Artist
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required string Name { get; set; }
  public string Description { get; set; } = "";
  public string PictureUrl { get; set; } = "";

  public ICollection<Song> Songs { get; set; } = [];
  public ICollection<Album> Albums { get; set; } = [];
}