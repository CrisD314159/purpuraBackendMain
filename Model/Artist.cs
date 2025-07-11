namespace purpuraMain.Model;

public class Artist
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required string Name { get; set; }
  public string Description { get; set; } = "";
  public string PictureUrl { get; set; } = "https://res.cloudinary.com/dw43hgf5p/image/upload/v1735614730/sd4xg3gxzsgtdiie0aht.jpg";

  public ICollection<Song> Songs { get; set; } = [];
  public ICollection<Album> Albums { get; set; } = [];
}