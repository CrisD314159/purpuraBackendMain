namespace purpuraMain.Model;

public class Artist
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public string? PictureUrl { get; set; }

  public ICollection<Song>? Songs { get; set; }
}