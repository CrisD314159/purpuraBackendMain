namespace purpuraMain.Model;

public class Genre
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public ICollection<Song> Songs { get; set; } = new List<Song>();

}